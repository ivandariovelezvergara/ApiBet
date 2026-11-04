using System.Globalization;
using System.Text;
using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.External.SharpApi.Contracts;
using Microsoft.Extensions.Logging;

namespace Infraestructure.External.SharpApi
{
    /// <summary>
    /// Cliente HTTP que consulta GET /opportunities/arbitrage en SharpAPI.
    /// </summary>
    internal sealed class SharpApiArbitrageClient : IArbitrageClient
    {
        private readonly SharpApiHttpGateway _gateway;
        private readonly ILogger<SharpApiArbitrageClient> _logger;

        /// <summary>
        /// Inicializa el cliente con la puerta de acceso HTTP compartida.
        /// </summary>
        public SharpApiArbitrageClient(
            SharpApiHttpGateway gateway,
            ILogger<SharpApiArbitrageClient> logger)
        {
            _gateway = gateway;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ArbitrageOpportunitiesPage> GetArbitrageAsync(
            int limit,
            int offset,
            string? sports,
            string? league,
            string? sportsbook,
            string? market,
            decimal? minProfit,
            bool? isLive,
            string? state,
            CancellationToken cancellationToken)
        {
            var path = BuildPath(limit, offset, sports, league, sportsbook, market, minProfit, isLive, state);
            _logger.LogInformation("Requesting SharpAPI {Path}.", path);

            var payload = await _gateway.GetJsonAsync<ArbitrageApiResponse>(
                path,
                "opportunities/arbitrage",
                cancellationToken);

            var items = payload.Data
                .Select(MapItem)
                .ToArray();

            _logger.LogInformation(
                "SharpAPI returned {ItemCount} arbitrage rows. HasMore: {HasMore}. NextOffset: {NextOffset}.",
                items.Length,
                payload.Pagination?.HasMore,
                payload.Pagination?.NextOffset);

            return new ArbitrageOpportunitiesPage(
                items,
                payload.Pagination?.HasMore ?? items.Length >= limit,
                payload.Pagination?.NextOffset,
                FlexibleUtcDateTimeParser.Parse(payload.UpdatedAt));
        }

        /// <summary>
        /// Arma la query de GET /opportunities/arbitrage.
        /// </summary>
        private static string BuildPath(
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
        /// Agrega un filtro CSV sin codificar la coma, como SharpAPI documenta (bwin,betsson).
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

        /// <summary>
        /// Convierte el contrato JSON en la entidad de dominio sin descartar filas.
        /// </summary>
        private static ArbitrageOpportunity MapItem(ArbitrageApiItem item)
        {
            var legs = item.Legs
                .Select(leg => new ArbitrageOpportunityLeg(
                    leg.DeepLink,
                    leg.ExternalEventId,
                    leg.ExternalMarketId,
                    leg.Line,
                    leg.MarketId,
                    leg.OddsAmerican,
                    leg.OddsDecimal,
                    leg.OddsId,
                    leg.OddsProbability,
                    leg.Selection,
                    leg.SelectionId,
                    leg.Sportsbook,
                    leg.StakePercent))
                .ToArray();

            var warnings = item.Warnings.Count == 0
                ? null
                : string.Join(',', item.Warnings.Where(warning => !string.IsNullOrWhiteSpace(warning)));

            return new ArbitrageOpportunity(
                item.Id,
                FlexibleUtcDateTimeParser.Parse(item.DetectedAt),
                item.EstimatedNetProfitPercent,
                item.EvAvailable,
                item.EvPercentage,
                item.EventId,
                item.EventName,
                item.ImpliedTotal,
                item.IsAlternateLine,
                item.IsLive,
                item.IsPlayerProp,
                item.League,
                item.LeagueLabel,
                item.Line,
                item.LineUnchanged,
                item.MarketLabel,
                item.MarketType,
                item.OldestOddsAgeSeconds,
                item.PlayerName,
                item.PossiblyStale,
                item.ProfitPercent,
                item.Sport,
                FlexibleUtcDateTimeParser.Parse(item.StartTime),
                item.StatCategory,
                warnings,
                legs);
        }
    }
}
