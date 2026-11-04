using Aplication.DTOs;
using Domain.Interfaces;

namespace Aplication.UseCases.GetLeagues
{
    /// <summary>
    /// Recupera el catálogo de ligas desde la base SQL Server local.
    /// </summary>
    public sealed class GetLeaguesUseCase : IGetLeaguesUseCase
    {
        private readonly ILeagueRepository _leagueRepository;

        /// <summary>
        /// Inicializa el caso de uso con el repositorio de ligas.
        /// </summary>
        /// <param name="leagueRepository">Repositorio local de ligas.</param>
        public GetLeaguesUseCase(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }

        /// <inheritdoc />
        public async Task<LeaguesCatalogDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            var catalog = await _leagueRepository.GetCatalogAsync(cancellationToken);
            return LeaguesCatalogDto.FromDomain(catalog);
        }
    }
}
