using System.Diagnostics;
using System.Text;
using Aplication.Configuration;
using Aplication.DTOs;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aplication.UseCases.SyncOdds
{
    /// <summary>
    /// Recorre GET /odds con cursor, persiste cada página en SQL Server y espera entre iteraciones.
    /// </summary>
    public sealed class SyncOddsUseCase : ISyncOddsUseCase
    {
        private readonly IOddsClient _oddsClient;
        private readonly IOddsRepository _oddsRepository;
        private readonly IOptionsSnapshot<OddsFeedOptions> _options;
        private readonly ILogger<SyncOddsUseCase> _logger;

        /// <summary>
        /// Inicializa el caso de uso con cliente, repositorio y opciones de paginación.
        /// </summary>
        public SyncOddsUseCase(
            IOddsClient oddsClient,
            IOddsRepository oddsRepository,
            IOptionsSnapshot<OddsFeedOptions> options,
            ILogger<SyncOddsUseCase> logger)
        {
            _oddsClient = oddsClient;
            _oddsRepository = oddsRepository;
            _options = options;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<OddsSyncResultDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var options = _options.Value;
            var sports = options.GetSportSlugs();
            var sportFilter = options.GetSportFilter();
            var marketFilter = options.GetMarketFilter();
            DateTime? sourceUpdatedAt = null;
            var pagesFetched = 0;
            var storedCount = 0;
            string? cursor = null;
            int? lastOffset = options.Offset;
            string? lastCursor = null;
            var delay = TimeSpan.FromSeconds(Math.Max(options.DelaySeconds, 0));
            var maxPages = Math.Max(options.MaxPages, 1);

            _logger.LogInformation(
                "Starting odds feed sync from OddsFeed. Sports: {Sports}. Market: {Market}. IsLive: {IsLive}. IsMainLine: {IsMainLine}. Limit {Limit}. Offset {Offset}. MaxPages {MaxPages}. Delay {DelaySeconds}s.",
                sportFilter ?? "(all)",
                marketFilter ?? "(all)",
                options.IsLive,
                options.IsMainLine,
                options.Limit,
                options.Offset,
                maxPages,
                delay.TotalSeconds);

            await _oddsRepository.ClearAllAsync(cancellationToken);

            for (var pageIndex = 0; pageIndex < maxPages; pageIndex++)
            {
                var offset = cursor is null ? lastOffset : null;

                _logger.LogInformation(
                    "Requesting odds page {Page}. Cursor present: {HasCursor}. Offset: {Offset}.",
                    pageIndex + 1,
                    !string.IsNullOrWhiteSpace(cursor),
                    offset);

                var page = await _oddsClient.GetOddsAsync(
                    options.Limit,
                    sportFilter,
                    marketFilter,
                    options.IsLive,
                    options.IsMainLine,
                    cursor,
                    offset,
                    cancellationToken);

                var savedInPage = await _oddsRepository.SavePageAsync(
                    page.Items,
                    page.SourceUpdatedAt,
                    cancellationToken);

                pagesFetched++;
                storedCount += savedInPage;
                sourceUpdatedAt = page.SourceUpdatedAt ?? sourceUpdatedAt;
                lastOffset = page.NextOffset ?? lastOffset;
                lastCursor = page.NextCursor ?? lastCursor;

                _logger.LogInformation(
                    "Stored {SavedCount} MarketOdds rows. HasMore: {HasMore}. NextOffset: {NextOffset}.",
                    savedInPage,
                    page.HasMore,
                    page.NextOffset);

                if (!page.HasMore || page.Items.Count == 0)
                {
                    _logger.LogInformation("Odds catalog has no more pages after page {Page}.", pageIndex + 1);
                    break;
                }

                cursor = string.IsNullOrWhiteSpace(page.NextCursor) ? null : page.NextCursor;
                if (cursor is null)
                {
                    lastOffset = page.NextOffset ?? ((lastOffset ?? 0) + options.Limit);
                }

                if (pageIndex + 1 < maxPages && delay > TimeSpan.Zero)
                {
                    _logger.LogInformation("Waiting {DelaySeconds} seconds before the next odds page.", delay.TotalSeconds);
                    await Task.Delay(delay, cancellationToken);
                }
            }

            stopwatch.Stop();
            var durationSeconds = Math.Round(stopwatch.Elapsed.TotalSeconds, 2);

            _logger.LogInformation(
                "Odds feed sync finished. Pages: {Pages}. Odds: {Count}. Last offset: {LastOffset}. Duration: {DurationSeconds}s.",
                pagesFetched,
                storedCount,
                lastOffset,
                durationSeconds);

            return new OddsSyncResultDto
            {
                StoredCount = storedCount,
                PagesFetched = pagesFetched,
                LastOffset = lastOffset,
                LastCursor = lastCursor,
                Limit = options.Limit,
                Offset = options.Offset,
                MaxPages = maxPages,
                DelaySeconds = options.DelaySeconds,
                Sports = sports,
                Market = marketFilter,
                IsLive = options.IsLive,
                IsMainLine = options.IsMainLine,
                SharpApiPath = BuildSharpApiPath(
                    options.Limit,
                    sportFilter,
                    marketFilter,
                    options.IsLive,
                    options.IsMainLine,
                    options.Offset),
                UpdatedAt = sourceUpdatedAt,
                LastSyncedAtUtc = DateTime.Now,
                DurationSeconds = durationSeconds
            };
        }

        /// <summary>
        /// Replica la query de SharpAPI con los valores de OddsFeed.
        /// </summary>
        private static string BuildSharpApiPath(
            int limit,
            string? sports,
            string? market,
            bool? isLive,
            bool? isMainLine,
            int offset)
        {
            var path = new StringBuilder()
                .Append("odds?limit=").Append(limit);

            AppendCsvQuery(path, "sport", sports);
            AppendCsvQuery(path, "market", market);

            if (isLive.HasValue)
            {
                path.Append("&is_live=").Append(isLive.Value.ToString().ToLowerInvariant());
            }

            if (isMainLine.HasValue)
            {
                path.Append("&is_main_line=").Append(isMainLine.Value.ToString().ToLowerInvariant());
            }

            path.Append("&offset=").Append(offset);
            return path.ToString();
        }

        /// <summary>
        /// Agrega un filtro CSV sin codificar la coma.
        /// </summary>
        private static void AppendCsvQuery(StringBuilder path, string name, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            var encoded = string.Join(
                ',',
                value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(Uri.EscapeDataString));

            if (encoded.Length == 0)
            {
                return;
            }

            path.Append('&').Append(name).Append('=').Append(encoded);
        }
    }
}
