using System.Diagnostics;
using System.Text;
using Aplication.Configuration;
using Aplication.DTOs;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aplication.UseCases.SyncBestOdds
{
    /// <summary>
    /// Recorre odds/best por offset, persiste cada página en SQL Server y espera entre iteraciones.
    /// </summary>
    public sealed class SyncBestOddsUseCase : ISyncBestOddsUseCase
    {
        private readonly IBestOddsClient _bestOddsClient;
        private readonly IBestOddsRepository _bestOddsRepository;
        private readonly IOptionsSnapshot<OddsSyncOptions> _options;
        private readonly ILogger<SyncBestOddsUseCase> _logger;

        /// <summary>
        /// Inicializa el caso de uso con cliente, repositorio y opciones de paginación.
        /// </summary>
        public SyncBestOddsUseCase(
            IBestOddsClient bestOddsClient,
            IBestOddsRepository bestOddsRepository,
            IOptionsSnapshot<OddsSyncOptions> options,
            ILogger<SyncBestOddsUseCase> logger)
        {
            _bestOddsClient = bestOddsClient;
            _bestOddsRepository = bestOddsRepository;
            _options = options;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<BestOddsSyncResultDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var options = _options.Value;
            var sports = options.GetSportSlugs();
            var sportFilter = options.GetSportFilter();
            if (string.IsNullOrWhiteSpace(sportFilter))
            {
                throw new InvalidOperationException("OddsSync:Sports must contain at least one sport.");
            }

            var marketFilter = options.GetMarketFilter();
            DateTime? sourceUpdatedAt = null;
            var pagesFetched = 0;
            var storedCount = 0;
            var storedBookCount = 0;
            var lastOffset = options.Offset;
            var delay = TimeSpan.FromSeconds(Math.Max(options.DelaySeconds, 0));

            _logger.LogInformation(
                "Starting best-odds sync from OddsSync. Sports: {Sports}. Market: {Market}. IsMainLine: {IsMainLine}. Offset {Offset} to {MaxOffset}. Limit {Limit}. Delay {DelaySeconds}s.",
                sportFilter,
                marketFilter ?? "(all)",
                options.IsMainLine,
                options.Offset,
                options.MaxOffset,
                options.Limit,
                delay.TotalSeconds);

            await _bestOddsRepository.ClearAllAsync(cancellationToken);

            for (var offset = options.Offset; offset <= options.MaxOffset; offset += Math.Max(options.OffsetIncrement, 1))
            {
                lastOffset = offset;
                _logger.LogInformation("Requesting best odds page. Offset: {Offset}.", offset);

                var page = await _bestOddsClient.GetBestOddsAsync(
                    options.Limit,
                    offset,
                    sportFilter,
                    marketFilter,
                    options.IsMainLine,
                    cancellationToken);

                var savedInPage = await _bestOddsRepository.SavePageAsync(
                    page.Items,
                    page.SourceUpdatedAt,
                    cancellationToken);

                pagesFetched++;
                storedCount += savedInPage.OddCount;
                storedBookCount += savedInPage.BookCount;
                sourceUpdatedAt = page.SourceUpdatedAt ?? sourceUpdatedAt;

                _logger.LogInformation(
                    "Stored {SavedCount} best-odds rows and {SavedBookCount} all_books rows for offset {Offset}.",
                    savedInPage.OddCount,
                    savedInPage.BookCount,
                    offset);

                if (!page.HasMore || page.Items.Count == 0)
                {
                    _logger.LogInformation("Best-odds catalog has no more pages after offset {Offset}.", offset);
                    break;
                }

                if (offset + Math.Max(options.OffsetIncrement, 1) <= options.MaxOffset && delay > TimeSpan.Zero)
                {
                    _logger.LogInformation("Waiting {DelaySeconds} seconds before the next offset.", delay.TotalSeconds);
                    await Task.Delay(delay, cancellationToken);
                }
            }

            stopwatch.Stop();
            var durationSeconds = Math.Round(stopwatch.Elapsed.TotalSeconds, 2);

            _logger.LogInformation(
                "Best-odds sync finished. Pages: {Pages}. Odds: {Count}. All books: {BookCount}. Last offset: {LastOffset}. Duration: {DurationSeconds}s.",
                pagesFetched,
                storedCount,
                storedBookCount,
                lastOffset,
                durationSeconds);

            return new BestOddsSyncResultDto
            {
                StoredCount = storedCount,
                StoredBookCount = storedBookCount,
                PagesFetched = pagesFetched,
                LastOffset = lastOffset,
                Limit = options.Limit,
                Offset = options.Offset,
                MaxOffset = options.MaxOffset,
                OffsetIncrement = options.OffsetIncrement,
                DelaySeconds = options.DelaySeconds,
                Sports = sports,
                Market = marketFilter,
                IsMainLine = options.IsMainLine,
                SharpApiPath = BuildSharpApiPath(
                    options.Limit,
                    options.Offset,
                    sportFilter,
                    marketFilter,
                    options.IsMainLine),
                UpdatedAt = sourceUpdatedAt,
                LastSyncedAtUtc = DateTime.Now,
                DurationSeconds = durationSeconds
            };
        }

        /// <summary>
        /// Replica la query de SharpAPI con los valores de OddsSync.
        /// </summary>
        private static string BuildSharpApiPath(
            int limit,
            int offset,
            string sports,
            string? market,
            bool? isMainLine)
        {
            var path = new StringBuilder()
                .Append("odds/best?limit=").Append(limit)
                .Append("&offset=").Append(offset);

            AppendCsvQuery(path, "sport", sports);
            AppendCsvQuery(path, "market", market);

            if (isMainLine.HasValue)
            {
                path.Append("&is_main_line=").Append(isMainLine.Value.ToString().ToLowerInvariant());
            }

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
