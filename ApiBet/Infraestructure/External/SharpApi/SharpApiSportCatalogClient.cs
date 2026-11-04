using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.External.SharpApi.Contracts;
using Microsoft.Extensions.Logging;

namespace Infraestructure.External.SharpApi
{
    /// <summary>
    /// Cliente HTTP que consulta el catálogo de deportes en SharpAPI.
    /// </summary>
    internal sealed class SharpApiSportCatalogClient : ISportCatalogClient
    {
        private readonly SharpApiHttpGateway _gateway;
        private readonly ILogger<SharpApiSportCatalogClient> _logger;

        /// <summary>
        /// Inicializa el cliente con la puerta de acceso HTTP compartida.
        /// </summary>
        /// <param name="gateway">Puerta de acceso hacia SharpAPI.</param>
        /// <param name="logger">Registro de las llamadas al proveedor externo.</param>
        public SharpApiSportCatalogClient(
            SharpApiHttpGateway gateway,
            ILogger<SharpApiSportCatalogClient> logger)
        {
            _gateway = gateway;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<SportCatalog> GetSportsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Requesting sports catalog from SharpAPI.");

            var payload = await _gateway.GetJsonAsync<SportsApiResponse>(
                "sports",
                "sports",
                cancellationToken);

            var sports = payload.Data
                .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                .Select(item => new Sport(
                    item.Id,
                    string.IsNullOrWhiteSpace(item.Name) ? item.Id : item.Name,
                    item.NumericalId,
                    item.EventCount,
                    item.LiveCount,
                    item.Leagues))
                .ToArray();

            var sourceUpdatedAt = FlexibleUtcDateTimeParser.Parse(payload.UpdatedAt);

            _logger.LogInformation("SharpAPI returned {SportCount} sports.", sports.Length);

            return new SportCatalog(sports, sourceUpdatedAt);
        }
    }
}
