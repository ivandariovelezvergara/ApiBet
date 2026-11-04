using Aplication.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Aplication.UseCases.SyncSportsbooks
{
    /// <summary>
    /// Orquesta la obtención remota del catálogo de casas y su almacenamiento local.
    /// </summary>
    public sealed class SyncSportsbooksUseCase : ISyncSportsbooksUseCase
    {
        private readonly ISportsbookCatalogClient _sportsbookCatalogClient;
        private readonly ISportsbookRepository _sportsbookRepository;
        private readonly ILogger<SyncSportsbooksUseCase> _logger;

        /// <summary>
        /// Inicializa el caso de uso con sus dependencias de dominio.
        /// </summary>
        /// <param name="sportsbookCatalogClient">Cliente del catálogo externo de casas.</param>
        /// <param name="sportsbookRepository">Repositorio local de casas.</param>
        /// <param name="logger">Registro de la operación de sincronización.</param>
        public SyncSportsbooksUseCase(
            ISportsbookCatalogClient sportsbookCatalogClient,
            ISportsbookRepository sportsbookRepository,
            ILogger<SyncSportsbooksUseCase> logger)
        {
            _sportsbookCatalogClient = sportsbookCatalogClient;
            _sportsbookRepository = sportsbookRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<SportsbooksCatalogDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting SharpAPI sportsbooks catalog synchronization.");

            var remoteCatalog = await _sportsbookCatalogClient.GetSportsbooksAsync(cancellationToken);
            var catalogToPersist = new SportsbookCatalog(
                remoteCatalog.Sportsbooks,
                remoteCatalog.SourceUpdatedAt,
                DateTime.Now);

            await _sportsbookRepository.SaveCatalogAsync(catalogToPersist, cancellationToken);
            var persistedCatalog = await _sportsbookRepository.GetCatalogAsync(cancellationToken);

            _logger.LogInformation(
                "Sportsbooks catalog synchronized. Sportsbooks stored: {SportsbookCount}.",
                persistedCatalog.Sportsbooks.Count);

            return SportsbooksCatalogDto.FromDomain(persistedCatalog);
        }
    }
}
