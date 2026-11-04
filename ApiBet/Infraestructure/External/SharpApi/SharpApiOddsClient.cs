using System.Text;
using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.External.SharpApi.Contracts;
using Microsoft.Extensions.Logging;

namespace Infraestructure.External.SharpApi
{
    /// <summary>
    /// Cliente HTTP que consulta GET /odds y GET /odds/delta en SharpAPI.
    /// </summary>
    internal sealed class SharpApiOddsClient : IOddsClient
    {
        private readonly SharpApiHttpGateway _gateway;
        private readonly ILogger<SharpApiOddsClient> _logger;

        /// <summary>
        /// Inicializa el cliente con la puerta de acceso HTTP compartida.
        /// </summary>
        public SharpApiOddsClient(
            SharpApiHttpGateway gateway,
            ILogger<SharpApiOddsClient> logger)
        {
            _gateway = gateway;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<MarketOddsPage> GetOddsAsync(
            int limit,
            string? sports,
            string? market,
            bool? isLive,
            bool? isMainLine,
            string? cursor,
            int? offset,
            CancellationToken cancellationToken)
        {
            var path = BuildPath(limit, sports, market, isLive, isMainLine, cursor, offset);
            _logger.LogInformation("Requesting SharpAPI {Path}.", TruncateForLog(path));

            var payload = await _gateway.GetJsonAsync<OddsApiResponse>(
                path,
                "odds",
                cancellationToken);

            var items = payload.Data
                .Select(MapItem)
                .ToArray();

            _logger.LogInformation(
                "SharpAPI returned {ItemCount} odds rows. HasMore: {HasMore}. NextOffset: {NextOffset}.",
                items.Length,
                payload.Pagination?.HasMore,
                payload.Pagination?.NextOffset);

            return new MarketOddsPage(
                items,
                payload.Pagination?.HasMore ?? items.Length >= limit,
                payload.Pagination?.NextOffset,
                payload.Pagination?.NextCursor,
                FlexibleUtcDateTimeParser.Parse(payload.UpdatedAt));
        }

        /// <inheritdoc />
        public async Task<MarketOddsDeltaPage> GetOddsDeltaAsync(
            string since,
            int limit,
            string? sports,
            string? market,
            bool? isLive,
            bool? isMainLine,
            string? cursor,
            int? offset,
            CancellationToken cancellationToken)
        {
            var path = BuildDeltaPath(since, limit, sports, market, isLive, isMainLine, cursor, offset);
            _logger.LogInformation("Requesting SharpAPI {Path}.", TruncateForLog(path));

            var payload = await _gateway.GetJsonAsync<OddsDeltaApiResponse>(
                path,
                "odds delta",
                cancellationToken);

            var items = (payload.Data ?? [])
                .Select(MapItem)
                .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                .ToArray();

            var removed = payload.Removed?
                .Select(MapRemoved)
                .Where(item => item is not null)
                .Cast<RemovedOdd>()
                .ToArray() ?? [];

            var hasMore = payload.Pagination?.HasMore ?? items.Length >= limit;

            _logger.LogInformation(
                "SharpAPI delta returned {ItemCount} odds rows and {RemovedCount} removed ids. HasMore: {HasMore}. NextOffset: {NextOffset}.",
                items.Length,
                removed.Length,
                hasMore,
                payload.Pagination?.NextOffset);

            return new MarketOddsDeltaPage(
                items,
                removed,
                hasMore,
                payload.Pagination?.NextOffset,
                payload.Pagination?.NextCursor,
                FlexibleUtcDateTimeParser.Parse(payload.UpdatedAt),
                payload.Overflow,
                payload.RemovedTruncated,
                payload.SinceClamped);
        }

        /// <summary>
        /// Arma la query de GET /odds. El cursor tiene prioridad sobre offset.
        /// </summary>
        private static string BuildPath(
            int limit,
            string? sports,
            string? market,
            bool? isLive,
            bool? isMainLine,
            string? cursor,
            int? offset)
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

            if (!string.IsNullOrWhiteSpace(cursor))
            {
                path.Append("&cursor=").Append(Uri.EscapeDataString(cursor.Trim()));
            }
            else if (offset.HasValue)
            {
                path.Append("&offset=").Append(offset.Value);
            }

            return path.ToString();
        }

        /// <summary>
        /// Arma la query de GET /odds/delta. El cursor tiene prioridad sobre offset.
        /// </summary>
        private static string BuildDeltaPath(
            string since,
            int limit,
            string? sports,
            string? market,
            bool? isLive,
            bool? isMainLine,
            string? cursor,
            int? offset)
        {
            var path = new StringBuilder("odds/delta?since=")
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

            if (!string.IsNullOrWhiteSpace(cursor))
            {
                path.Append("&cursor=").Append(Uri.EscapeDataString(cursor.Trim()));
            }
            else if (offset.HasValue)
            {
                path.Append("&offset=").Append(offset.Value);
            }

            return path.ToString();
        }

        /// <summary>
        /// Convierte un retiro del contrato JSON; omite identificadores vacíos.
        /// </summary>
        private static RemovedOdd? MapRemoved(OddsDeltaRemovedApiItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                return null;
            }

            return new RemovedOdd(
                item.Id,
                item.Sportsbook,
                FlexibleUtcDateTimeParser.Parse(item.RemovedAt),
                FlexibleUtcDateTimeParser.Parse(item.EventStart),
                item.WasLive ?? false,
                item.Boundary ?? false);
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

        /// <summary>
        /// Convierte el contrato JSON en la entidad de dominio sin descartar filas.
        /// </summary>
        private static MarketOdd MapItem(OddsApiItem item)
        {
            return new MarketOdd(
                item.Id,
                item.Sportsbook,
                item.EventId,
                item.EventUuid,
                item.Sport,
                item.League,
                item.HomeTeam,
                item.AwayTeam,
                item.MarketType,
                item.Selection,
                item.SelectionType,
                item.TeamSide,
                item.OddsAmerican,
                item.OddsDecimal,
                item.OddsProbability,
                item.Line,
                FlexibleUtcDateTimeParser.Parse(item.EventStartTime),
                item.IsLive,
                item.IsActive,
                FlexibleUtcDateTimeParser.Parse(item.Timestamp),
                item.DeepLink,
                item.MarketId,
                item.SelectionId,
                item.ExternalMarketId,
                item.IsAlternateLine,
                item.IsMainLine,
                item.IsStalePregamePrice,
                item.IsPlayerProp,
                item.IsImpossibleScoreline,
                item.SeasonType,
                item.Home?.Id,
                item.Home?.NumericalId,
                item.Home?.Name,
                item.Home?.Logo,
                item.Home?.Abbreviation,
                item.Home?.City,
                item.Home?.Mascot,
                item.Home?.Conference,
                item.Home?.Division,
                item.Away?.Id,
                item.Away?.NumericalId,
                item.Away?.Name,
                item.Away?.Logo,
                item.Away?.Abbreviation,
                item.Away?.City,
                item.Away?.Mascot,
                item.Away?.Conference,
                item.Away?.Division,
                item.SportRef?.Id,
                item.SportRef?.Name,
                item.SportRef?.NumericalId,
                item.LeagueRef?.Id,
                item.LeagueRef?.Label,
                item.LeagueRef?.NumericalId,
                item.MarketRef?.Id,
                item.MarketRef?.Label,
                item.MarketRef?.NumericalId,
                item.SportsbookRef?.Id,
                item.SportsbookRef?.Label,
                item.SportsbookRef?.NumericalId,
                item.CompetitionRef?.Id,
                item.CompetitionRef?.Label,
                item.CompetitionRef?.NumericalId,
                item.ExternalEventId);
        }

        /// <summary>
        /// Recorta el path para no volcar el cursor completo en los logs.
        /// </summary>
        private static string TruncateForLog(string path)
        {
            const int maxLength = 220;
            return path.Length <= maxLength ? path : path[..maxLength] + "...";
        }
    }
}
