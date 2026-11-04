using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.External.SharpApi.Contracts;
using Microsoft.Extensions.Logging;

namespace Infraestructure.External.SharpApi
{
    /// <summary>
    /// Cliente HTTP que consulta el catálogo de mercados en SharpAPI.
    /// </summary>
    internal sealed class SharpApiMarketCatalogClient : IMarketCatalogClient
    {
        private readonly SharpApiHttpGateway _gateway;
        private readonly ILogger<SharpApiMarketCatalogClient> _logger;

        /// <summary>
        /// Inicializa el cliente con la puerta de acceso HTTP compartida.
        /// </summary>
        /// <param name="gateway">Puerta de acceso hacia SharpAPI.</param>
        /// <param name="logger">Registro de las llamadas al proveedor externo.</param>
        public SharpApiMarketCatalogClient(
            SharpApiHttpGateway gateway,
            ILogger<SharpApiMarketCatalogClient> logger)
        {
            _gateway = gateway;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<MarketCatalog> GetMarketsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Requesting markets catalog from SharpAPI.");

            var payload = await _gateway.GetJsonAsync<MarketsApiResponse>(
                "markets",
                "markets",
                cancellationToken);

            var markets = payload.Data
                .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                .Select(item => new Market(
                    item.Id,
                    string.IsNullOrWhiteSpace(item.Name) ? item.Id : item.Name,
                    item.NumericalId,
                    item.EventCount,
                    item.Sports))
                .ToArray();

            var sourceUpdatedAt = FlexibleUtcDateTimeParser.Parse(payload.UpdatedAt);

            _logger.LogInformation("SharpAPI returned {MarketCount} markets.", markets.Length);

            return new MarketCatalog(markets, sourceUpdatedAt);
        }
    }
}
