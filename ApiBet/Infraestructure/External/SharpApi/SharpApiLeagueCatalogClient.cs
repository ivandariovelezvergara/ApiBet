using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.External.SharpApi.Contracts;
using Microsoft.Extensions.Logging;

namespace Infraestructure.External.SharpApi
{
    /// <summary>
    /// Cliente HTTP que consulta el catálogo de ligas en SharpAPI.
    /// </summary>
    internal sealed class SharpApiLeagueCatalogClient : ILeagueCatalogClient
    {
        private readonly SharpApiHttpGateway _gateway;
        private readonly ILogger<SharpApiLeagueCatalogClient> _logger;

        /// <summary>
        /// Inicializa el cliente con la puerta de acceso HTTP compartida.
        /// </summary>
        /// <param name="gateway">Puerta de acceso hacia SharpAPI.</param>
        /// <param name="logger">Registro de las llamadas al proveedor externo.</param>
        public SharpApiLeagueCatalogClient(
            SharpApiHttpGateway gateway,
            ILogger<SharpApiLeagueCatalogClient> logger)
        {
            _gateway = gateway;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<LeagueCatalog> GetLeaguesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Requesting leagues catalog from SharpAPI.");

            var payload = await _gateway.GetJsonAsync<LeaguesApiResponse>(
                "leagues",
                "leagues",
                cancellationToken);

            var leagues = payload.Data
                .Where(item => !string.IsNullOrWhiteSpace(item.Id) && !string.IsNullOrWhiteSpace(item.Sport))
                .Select(item => new League(
                    item.Id,
                    string.IsNullOrWhiteSpace(item.DisplayName) ? item.Id : item.DisplayName,
                    item.NumericalId,
                    item.Sport,
                    item.EventCount,
                    item.LiveCount))
                .ToArray();

            var sourceUpdatedAt = FlexibleUtcDateTimeParser.Parse(payload.UpdatedAt);

            _logger.LogInformation("SharpAPI returned {LeagueCount} leagues.", leagues.Length);

            return new LeagueCatalog(leagues, sourceUpdatedAt);
        }
    }
}
