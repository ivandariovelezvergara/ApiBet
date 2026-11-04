using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.External.SharpApi.Contracts;
using Microsoft.Extensions.Logging;

namespace Infraestructure.External.SharpApi
{
    /// <summary>
    /// Cliente HTTP que consulta el catálogo de casas de apuestas en SharpAPI.
    /// </summary>
    internal sealed class SharpApiSportsbookCatalogClient : ISportsbookCatalogClient
    {
        private readonly SharpApiHttpGateway _gateway;
        private readonly ILogger<SharpApiSportsbookCatalogClient> _logger;

        /// <summary>
        /// Inicializa el cliente con la puerta de acceso HTTP compartida.
        /// </summary>
        /// <param name="gateway">Puerta de acceso hacia SharpAPI.</param>
        /// <param name="logger">Registro de las llamadas al proveedor externo.</param>
        public SharpApiSportsbookCatalogClient(
            SharpApiHttpGateway gateway,
            ILogger<SharpApiSportsbookCatalogClient> logger)
        {
            _gateway = gateway;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<SportsbookCatalog> GetSportsbooksAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Requesting sportsbooks catalog from SharpAPI.");

            var payload = await _gateway.GetJsonAsync<SportsbooksApiResponse>(
                "sportsbooks",
                "sportsbooks",
                cancellationToken);

            var sportsbooks = payload.Data
                .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                .Select(item => new Sportsbook(
                    item.Id,
                    item.NumericalId,
                    string.IsNullOrWhiteSpace(item.DisplayName) ? item.Id : item.DisplayName,
                    item.ShortName,
                    item.Category,
                    item.HasLiveOdds,
                    item.HasPlayerProps,
                    item.HasLimits,
                    item.Regions,
                    item.IsSharp,
                    item.Status,
                    item.EventCount,
                    FlexibleUtcDateTimeParser.Parse(item.LastUpdate),
                    item.RequiresTier,
                    item.ComingSoon))
                .ToArray();

            var sourceUpdatedAt = FlexibleUtcDateTimeParser.Parse(payload.UpdatedAt);

            _logger.LogInformation("SharpAPI returned {SportsbookCount} sportsbooks.", sportsbooks.Length);

            return new SportsbookCatalog(sportsbooks, sourceUpdatedAt);
        }
    }
}
