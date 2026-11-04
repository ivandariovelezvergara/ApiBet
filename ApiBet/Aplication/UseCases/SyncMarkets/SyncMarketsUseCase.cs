using Aplication.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Aplication.UseCases.SyncMarkets
{
    /// <summary>
    /// Orquesta la obtención remota del catálogo de mercados y su almacenamiento local.
    /// </summary>
    public sealed class SyncMarketsUseCase : ISyncMarketsUseCase
    {
        private readonly IMarketCatalogClient _marketCatalogClient;
        private readonly IMarketRepository _marketRepository;
        private readonly ILogger<SyncMarketsUseCase> _logger;

        /// <summary>
        /// Inicializa el caso de uso con sus dependencias de dominio.
        /// </summary>
        /// <param name="marketCatalogClient">Cliente del catálogo externo de mercados.</param>
        /// <param name="marketRepository">Repositorio local de mercados.</param>
        /// <param name="logger">Registro de la operación de sincronización.</param>
        public SyncMarketsUseCase(
            IMarketCatalogClient marketCatalogClient,
            IMarketRepository marketRepository,
            ILogger<SyncMarketsUseCase> logger)
        {
            _marketCatalogClient = marketCatalogClient;
            _marketRepository = marketRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<MarketsCatalogDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting SharpAPI markets catalog synchronization.");

            var remoteCatalog = await _marketCatalogClient.GetMarketsAsync(cancellationToken);
            var catalogToPersist = new MarketCatalog(
                remoteCatalog.Markets,
                remoteCatalog.SourceUpdatedAt,
                DateTime.Now);

            await _marketRepository.SaveCatalogAsync(catalogToPersist, cancellationToken);
            var persistedCatalog = await _marketRepository.GetCatalogAsync(cancellationToken);

            _logger.LogInformation(
                "Markets catalog synchronized. Markets stored: {MarketCount}.",
                persistedCatalog.Markets.Count);

            return MarketsCatalogDto.FromDomain(persistedCatalog);
        }
    }
}
