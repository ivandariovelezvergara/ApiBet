using System.Diagnostics;
using System.Globalization;
using System.Text;
using Aplication.Configuration;
using Aplication.DTOs;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aplication.UseCases.SyncArbitrage
{
    /// <summary>
    /// Recorre GET /opportunities/arbitrage por offset y persiste cada página en SQL Server.
    /// </summary>
    public sealed class SyncArbitrageUseCase : ISyncArbitrageUseCase
    {
        private static readonly SemaphoreSlim SyncGate = new(1, 1);

        private readonly IArbitrageClient _arbitrageClient;
        private readonly IArbitrageOpportunityRepository _arbitrageRepository;
        private readonly IOptionsSnapshot<ArbitrageFeedOptions> _options;
        private readonly ILogger<SyncArbitrageUseCase> _logger;

        /// <summary>
        /// Inicializa el caso de uso con cliente, repositorio y opciones de paginación.
        /// </summary>
        public SyncArbitrageUseCase(
            IArbitrageClient arbitrageClient,
            IArbitrageOpportunityRepository arbitrageRepository,
            IOptionsSnapshot<ArbitrageFeedOptions> options,
            ILogger<SyncArbitrageUseCase> logger)
        {
            _arbitrageClient = arbitrageClient;
            _arbitrageRepository = arbitrageRepository;
            _options = options;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ArbitrageSyncResultDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            await SyncGate.WaitAsync(cancellationToken);
            try
            {
                return await ExecuteCoreAsync(cancellationToken);
            }
            finally
            {
                SyncGate.Release();
            }
        }

        /// <summary>
        /// Ejecuta la paginación y la persistencia con el candado de sync ya tomado.
        /// </summary>
        private async Task<ArbitrageSyncResultDto> ExecuteCoreAsync(CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var options = _options.Value;
            var sports = options.GetSportSlugs();
            var sportFilter = options.GetSportFilter();
            var leagueFilter = options.GetLeagueFilter();
            var sportsbookFilter = options.GetSportsbookFilter();
            var marketFilter = options.GetMarketFilter();
            var stateFilter = string.IsNullOrWhiteSpace(options.State) ? null : options.State.Trim();
            DateTime? sourceUpdatedAt = null;
            var pagesFetched = 0;
            var storedCount = 0;
            var storedLegCount = 0;
            var lastOffset = options.Offset;
            var delay = TimeSpan.FromSeconds(Math.Max(options.DelaySeconds, 0));
            var maxPages = Math.Max(options.MaxPages, 1);
            var retentionMinutes = Math.Max(options.RetentionMinutes, 0);

            _logger.LogInformation(
                "Starting arbitrage sync from ArbitrageFeed. Sports: {Sports}. League: {League}. Sportsbook: {Sportsbook}. Market: {Market}. MinProfit: {MinProfit}. IsLive: {IsLive}. State: {State}. Limit {Limit}. Offset {Offset}. MaxPages {MaxPages}. Delay {DelaySeconds}s. RetentionMinutes: {RetentionMinutes}.",
                sportFilter ?? "(all)",
                leagueFilter ?? "(all)",
                sportsbookFilter ?? "(all)",
                marketFilter ?? "(all)",
                options.MinProfit,
                options.IsLive,
                stateFilter ?? "(default)",
                options.Limit,
                lastOffset,
                maxPages,
                delay.TotalSeconds,
                retentionMinutes);

            var deletedCount = await _arbitrageRepository.ClearExpiredAsync(retentionMinutes, cancellationToken);
            _logger.LogInformation(
                "Cleared {DeletedCount} arbitrage opportunities older than {RetentionMinutes} minutes.",
                deletedCount,
                retentionMinutes);

            for (var pageIndex = 0; pageIndex < maxPages; pageIndex++)
            {
                _logger.LogInformation("Requesting arbitrage page {Page}. Offset: {Offset}.", pageIndex + 1, lastOffset);

                var page = await _arbitrageClient.GetArbitrageAsync(
                    options.Limit,
                    lastOffset,
                    sportFilter,
                    leagueFilter,
                    sportsbookFilter,
                    marketFilter,
                    options.MinProfit,
                    options.IsLive,
                    stateFilter,
                    cancellationToken);

                var savedInPage = await _arbitrageRepository.SavePageAsync(
                    page.Items,
                    page.SourceUpdatedAt,
                    sportsbookFilter,
                    cancellationToken);

                pagesFetched++;
                storedCount += savedInPage.OpportunityCount;
                storedLegCount += savedInPage.LegCount;
                sourceUpdatedAt = page.SourceUpdatedAt ?? sourceUpdatedAt;

                _logger.LogInformation(
                    "Stored {SavedCount} arbitrage rows and {SavedLegCount} legs. HasMore: {HasMore}. NextOffset: {NextOffset}.",
                    savedInPage.OpportunityCount,
                    savedInPage.LegCount,
                    page.HasMore,
                    page.NextOffset);

                if (!page.HasMore || page.Items.Count == 0)
                {
                    _logger.LogInformation("Arbitrage catalog has no more pages after page {Page}.", pageIndex + 1);
                    break;
                }

                lastOffset = page.NextOffset ?? (lastOffset + options.Limit);

                if (pageIndex + 1 < maxPages && delay > TimeSpan.Zero)
                {
                    _logger.LogInformation(
                        "Waiting {DelaySeconds} seconds before the next arbitrage page.",
                        delay.TotalSeconds);
                    await Task.Delay(delay, cancellationToken);
                }
            }

            stopwatch.Stop();
            var durationSeconds = Math.Round(stopwatch.Elapsed.TotalSeconds, 2);

            _logger.LogInformation(
                "Arbitrage sync finished. Pages: {Pages}. Opportunities: {Count}. Legs: {LegCount}. Last offset: {LastOffset}. Duration: {DurationSeconds}s.",
                pagesFetched,
                storedCount,
                storedLegCount,
                lastOffset,
                durationSeconds);

            return new ArbitrageSyncResultDto
            {
                StoredCount = storedCount,
                StoredLegCount = storedLegCount,
                DeletedCount = deletedCount,
                PagesFetched = pagesFetched,
                LastOffset = lastOffset,
                Limit = options.Limit,
                Offset = options.Offset,
                MaxPages = maxPages,
                DelaySeconds = options.DelaySeconds,
                RetentionMinutes = retentionMinutes,
                Sports = sports,
                League = leagueFilter,
                Sportsbook = sportsbookFilter,
                Market = marketFilter,
                MinProfit = options.MinProfit,
                IsLive = options.IsLive,
                State = stateFilter,
                SharpApiPath = BuildSharpApiPath(
                    options.Limit,
                    options.Offset,
                    sportFilter,
                    leagueFilter,
                    sportsbookFilter,
                    marketFilter,
                    options.MinProfit,
                    options.IsLive,
                    stateFilter),
                UpdatedAt = sourceUpdatedAt,
                LastSyncedAtUtc = DateTime.Now,
                DurationSeconds = durationSeconds
            };
        }

        /// <summary>
        /// Replica la query de SharpAPI con los valores de ArbitrageFeed.
        /// </summary>
        private static string BuildSharpApiPath(
            int limit,
            int offset,
            string? sports,
            string? league,
            string? sportsbook,
            string? market,
            decimal? minProfit,
            bool? isLive,
            string? state)
        {
            var path = new StringBuilder()
                .Append("opportunities/arbitrage?limit=").Append(limit)
                .Append("&offset=").Append(offset);

            AppendCsvQuery(path, "sport", sports);
            AppendCsvQuery(path, "league", league);
            AppendCsvQuery(path, "sportsbook", sportsbook);
            AppendCsvQuery(path, "market", market);

            if (minProfit.HasValue)
            {
                path.Append("&min_profit=").Append(minProfit.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (isLive.HasValue)
            {
                path.Append("&is_live=").Append(isLive.Value.ToString().ToLowerInvariant());
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                path.Append("&state=").Append(Uri.EscapeDataString(state.Trim()));
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
