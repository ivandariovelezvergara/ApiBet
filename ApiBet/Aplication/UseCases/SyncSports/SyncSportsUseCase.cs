using Aplication.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Aplication.UseCases.SyncSports
{
    /// <summary>
    /// Orquesta la obtención remota del catálogo y su almacenamiento local.
    /// </summary>
    public sealed class SyncSportsUseCase : ISyncSportsUseCase
    {
        private readonly ISportCatalogClient _sportCatalogClient;
        private readonly ISportRepository _sportRepository;
        private readonly ILogger<SyncSportsUseCase> _logger;

        /// <summary>
        /// Inicializa el caso de uso con sus dependencias de dominio.
        /// </summary>
        /// <param name="sportCatalogClient">Cliente del catálogo externo.</param>
        /// <param name="sportRepository">Repositorio local de deportes.</param>
        /// <param name="logger">Registro de la operación de sincronización.</param>
        public SyncSportsUseCase(
            ISportCatalogClient sportCatalogClient,
            ISportRepository sportRepository,
            ILogger<SyncSportsUseCase> logger)
        {
            _sportCatalogClient = sportCatalogClient;
            _sportRepository = sportRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<SportsCatalogDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting SharpAPI sports catalog synchronization.");

            var remoteCatalog = await _sportCatalogClient.GetSportsAsync(cancellationToken);
            var catalogToPersist = new SportCatalog(
                remoteCatalog.Sports,
                remoteCatalog.SourceUpdatedAt,
                DateTime.Now);

            await _sportRepository.SaveCatalogAsync(catalogToPersist, cancellationToken);
            var persistedCatalog = await _sportRepository.GetCatalogAsync(cancellationToken);

            _logger.LogInformation(
                "Sports catalog synchronized. Sports stored: {SportCount}.",
                persistedCatalog.Sports.Count);

            return SportsCatalogDto.FromDomain(persistedCatalog);
        }
    }
}
