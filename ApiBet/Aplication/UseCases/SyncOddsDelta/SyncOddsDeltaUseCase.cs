using System.Diagnostics;
using System.Text;
using Aplication.Configuration;
using Aplication.DTOs;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aplication.UseCases.SyncOddsDelta
{
    /// <summary>
    /// Recorre GET /odds/delta con cursor, persiste cada lote incremental y espera entre iteraciones.
    /// </summary>
    public sealed class SyncOddsDeltaUseCase : ISyncOddsDeltaUseCase
    {
        private readonly IOddsClient _oddsClient;
        private readonly IOddsRepository _oddsRepository;
        private readonly IOptionsSnapshot<OddsDeltaOptions> _options;
        private readonly ILogger<SyncOddsDeltaUseCase> _logger;

        /// <summary>
        /// Inicializa el caso de uso con cliente, repositorio y opciones de paginación delta.
        /// </summary>
        public SyncOddsDeltaUseCase(
            IOddsClient oddsClient,
            IOddsRepository oddsRepository,
            IOptionsSnapshot<OddsDeltaOptions> options,
            ILogger<SyncOddsDeltaUseCase> logger)
        {
            _oddsClient = oddsClient;
            _oddsRepository = oddsRepository;
            _options = options;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<OddsDeltaSyncResultDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var options = _options.Value;
            var since = options.GetSinceUtc();
            var sports = options.GetSportSlugs();
            var sportFilter = options.GetSportFilter();
            var marketFilter = options.GetMarketFilter();
            DateTime? sourceUpdatedAt = null;
            var pagesFetched = 0;
            var upsertedCount = 0;
            var deletedCount = 0;
            var overflow = false;
            var removedTruncated = false;
            var sinceClamped = false;
            string? cursor = null;
            int? lastOffset = options.Offset;
            string? lastCursor = null;
            var delay = TimeSpan.FromSeconds(Math.Max(options.DelaySeconds, 0));
            var maxPages = Math.Max(options.MaxPages, 1);

            _logger.LogInformation(
                "Starting odds delta sync from OddsDelta. Since: {Since}. Sports: {Sports}. Market: {Market}. IsLive: {IsLive}. IsMainLine: {IsMainLine}. Limit {Limit}. Offset {Offset}. MaxPages {MaxPages}. Delay {DelaySeconds}s.",
                since,
                sportFilter ?? "(all)",
                marketFilter ?? "(all)",
                options.IsLive,
                options.IsMainLine,
                options.Limit,
                options.Offset,
                maxPages,
                delay.TotalSeconds);

            await _oddsRepository.ClearDeltaAsync(cancellationToken);

            for (var pageIndex = 0; pageIndex < maxPages; pageIndex++)
            {
                var offset = cursor is null ? lastOffset : null;

                _logger.LogInformation(
                    "Requesting odds delta page {Page}. Cursor present: {HasCursor}. Offset: {Offset}.",
                    pageIndex + 1,
                    !string.IsNullOrWhiteSpace(cursor),
                    offset);

                var page = await _oddsClient.GetOddsDeltaAsync(
                    since,
                    options.Limit,
                    sportFilter,
                    marketFilter,
                    options.IsLive,
                    options.IsMainLine,
                    cursor,
                    offset,
                    cancellationToken);

                var persistResult = await _oddsRepository.SaveDeltaAsync(
                    page.Items,
                    page.Removed,
                    page.SourceUpdatedAt,
                    cancellationToken);

                pagesFetched++;
                upsertedCount += persistResult.UpsertedCount;
                deletedCount += persistResult.DeletedCount;
                overflow |= page.Overflow;
                removedTruncated |= page.RemovedTruncated;
                sinceClamped |= page.SinceClamped;
                sourceUpdatedAt = page.SourceUpdatedAt ?? sourceUpdatedAt;
                lastOffset = page.NextOffset ?? lastOffset;
                lastCursor = page.NextCursor ?? lastCursor;

                _logger.LogInformation(
                    "Delta page stored. Upserted: {UpsertedCount}. Deleted: {DeletedCount}. HasMore: {HasMore}. NextOffset: {NextOffset}.",
                    persistResult.UpsertedCount,
                    persistResult.DeletedCount,
                    page.HasMore,
                    page.NextOffset);

                if (!page.HasMore || (page.Items.Count == 0 && page.Removed.Count == 0))
                {
                    _logger.LogInformation("Odds delta has no more pages after page {Page}.", pageIndex + 1);
                    break;
                }

                cursor = string.IsNullOrWhiteSpace(page.NextCursor) ? null : page.NextCursor;
                if (cursor is null)
                {
                    lastOffset = page.NextOffset ?? ((lastOffset ?? 0) + options.Limit);
                }

                if (pageIndex + 1 < maxPages && delay > TimeSpan.Zero)
                {
                    _logger.LogInformation("Waiting {DelaySeconds} seconds before the next odds delta page.", delay.TotalSeconds);
                    await Task.Delay(delay, cancellationToken);
                }
            }

            stopwatch.Stop();
            var durationSeconds = Math.Round(stopwatch.Elapsed.TotalSeconds, 2);

            _logger.LogInformation(
                "Odds delta sync finished. Pages: {Pages}. Upserted: {UpsertedCount}. Deleted: {DeletedCount}. Last offset: {LastOffset}. Duration: {DurationSeconds}s.",
                pagesFetched,
                upsertedCount,
                deletedCount,
                lastOffset,
                durationSeconds);

            return new OddsDeltaSyncResultDto
            {
                UpsertedCount = upsertedCount,
                DeletedCount = deletedCount,
                PagesFetched = pagesFetched,
                LastOffset = lastOffset,
                LastCursor = lastCursor,
                Since = since,
                Limit = options.Limit,
                Offset = options.Offset,
                MaxPages = maxPages,
                DelaySeconds = options.DelaySeconds,
                Sports = sports,
                Market = marketFilter,
                IsLive = options.IsLive,
                IsMainLine = options.IsMainLine,
                SharpApiPath = BuildSharpApiPath(
                    since,
                    options.Limit,
                    sportFilter,
                    marketFilter,
                    options.IsLive,
                    options.IsMainLine,
                    options.Offset),
                Overflow = overflow,
                RemovedTruncated = removedTruncated,
                SinceClamped = sinceClamped,
                UpdatedAt = sourceUpdatedAt,
                LastSyncedAtUtc = DateTime.Now,
                DurationSeconds = durationSeconds
            };
        }

        /// <summary>
        /// Replica la query de SharpAPI con los valores de OddsDelta.
        /// </summary>
        private static string BuildSharpApiPath(
            string since,
            int limit,
            string? sports,
            string? market,
            bool? isLive,
            bool? isMainLine,
            int offset)
        {
            var path = new StringBuilder()
                .Append("odds/delta?since=")
                .Append(Uri.EscapeDataString(since))
                .Append("&limit=")
                .Append(limit);

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
