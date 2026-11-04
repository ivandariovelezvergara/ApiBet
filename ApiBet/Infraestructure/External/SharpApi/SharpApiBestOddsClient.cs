using System.Text;
using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.External.SharpApi.Contracts;
using Microsoft.Extensions.Logging;

namespace Infraestructure.External.SharpApi
{
    /// <summary>
    /// Cliente HTTP que consulta GET /odds/best en SharpAPI.
    /// </summary>
    internal sealed class SharpApiBestOddsClient : IBestOddsClient
    {
        private readonly SharpApiHttpGateway _gateway;
        private readonly ILogger<SharpApiBestOddsClient> _logger;

        /// <summary>
        /// Inicializa el cliente con la puerta de acceso HTTP compartida.
        /// </summary>
        public SharpApiBestOddsClient(
            SharpApiHttpGateway gateway,
            ILogger<SharpApiBestOddsClient> logger)
        {
            _gateway = gateway;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<BestOddsPage> GetBestOddsAsync(
            int limit,
            int offset,
            string sports,
            string? market,
            bool? isMainLine,
            CancellationToken cancellationToken)
        {
            var path = BuildPath(limit, offset, sports, market, isMainLine);

            _logger.LogInformation("Requesting SharpAPI {Path}.", path);

            var payload = await _gateway.GetJsonAsync<BestOddsApiResponse>(
                path,
                "odds/best",
                cancellationToken);

            var items = payload.Data
                .Select(MapItem)
                .ToArray();

            _logger.LogInformation(
                "SharpAPI returned {ItemCount} best-odds rows for offset {Offset}.",
                items.Length,
                offset);

            return new BestOddsPage(
                items,
                payload.Pagination?.HasMore ?? items.Length >= limit,
                payload.Pagination?.NextOffset,
                FlexibleUtcDateTimeParser.Parse(payload.UpdatedAt));
        }

        /// <summary>
        /// Arma la query de GET /odds/best con los filtros de OddsSync.
        /// </summary>
        private static string BuildPath(
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

        /// <summary>
        /// Convierte el contrato JSON en la entidad de dominio sin descartar filas.
        /// </summary>
        private static BestOdd MapItem(BestOddApiItem item)
        {
            var books = item.AllBooks
                .Select(book => new BestOddBook(
                    book.Book ?? book.Sportsbook ?? string.Empty,
                    book.Sportsbook ?? book.Book ?? string.Empty,
                    book.Odds?.American,
                    book.Odds?.Decimal,
                    book.Edge))
                .ToArray();

            return new BestOdd(
                item.EventId,
                item.EventName,
                item.Sport,
                item.League,
                item.MarketType,
                item.Selection,
                item.Line,
                item.IsMainLine,
                item.BestOdds?.American,
                item.BestOdds?.Decimal,
                item.BestOdds?.Probability,
                item.ConsensusOdds?.American,
                item.ConsensusOdds?.Decimal,
                item.ConsensusOdds?.Probability,
                item.MarketHold,
                item.BestBook,
                FlexibleUtcDateTimeParser.Parse(item.Timestamp),
                item.PlayerName,
                item.StatCategory,
                books);
        }
    }
}
