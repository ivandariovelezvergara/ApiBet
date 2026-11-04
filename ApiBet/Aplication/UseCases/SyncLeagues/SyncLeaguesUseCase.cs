using Aplication.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Aplication.UseCases.SyncLeagues
{
    /// <summary>
    /// Orquesta la obtención remota del catálogo de ligas y su almacenamiento local.
    /// </summary>
    public sealed class SyncLeaguesUseCase : ISyncLeaguesUseCase
    {
        private readonly ILeagueCatalogClient _leagueCatalogClient;
        private readonly ILeagueRepository _leagueRepository;
        private readonly ILogger<SyncLeaguesUseCase> _logger;

        /// <summary>
        /// Inicializa el caso de uso con sus dependencias de dominio.
        /// </summary>
        /// <param name="leagueCatalogClient">Cliente del catálogo externo de ligas.</param>
        /// <param name="leagueRepository">Repositorio local de ligas.</param>
        /// <param name="logger">Registro de la operación de sincronización.</param>
        public SyncLeaguesUseCase(
            ILeagueCatalogClient leagueCatalogClient,
            ILeagueRepository leagueRepository,
            ILogger<SyncLeaguesUseCase> logger)
        {
            _leagueCatalogClient = leagueCatalogClient;
            _leagueRepository = leagueRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<LeaguesCatalogDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting SharpAPI leagues catalog synchronization.");

            var remoteCatalog = await _leagueCatalogClient.GetLeaguesAsync(cancellationToken);
            var catalogToPersist = new LeagueCatalog(
                remoteCatalog.Leagues,
                remoteCatalog.SourceUpdatedAt,
                DateTime.Now);

            await _leagueRepository.SaveCatalogAsync(catalogToPersist, cancellationToken);
            var persistedCatalog = await _leagueRepository.GetCatalogAsync(cancellationToken);

            _logger.LogInformation(
                "Leagues catalog synchronized. Leagues stored: {LeagueCount}.",
                persistedCatalog.Leagues.Count);

            return LeaguesCatalogDto.FromDomain(persistedCatalog);
        }
    }
}
