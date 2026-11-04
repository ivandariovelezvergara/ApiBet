using Aplication.DTOs;
using Domain.Interfaces;

namespace Aplication.UseCases.GetSportsbooks
{
    /// <summary>
    /// Recupera el catálogo de casas de apuestas desde la base SQL Server local.
    /// </summary>
    public sealed class GetSportsbooksUseCase : IGetSportsbooksUseCase
    {
        private readonly ISportsbookRepository _sportsbookRepository;

        /// <summary>
        /// Inicializa el caso de uso con el repositorio de casas.
        /// </summary>
        /// <param name="sportsbookRepository">Repositorio local de casas de apuestas.</param>
        public GetSportsbooksUseCase(ISportsbookRepository sportsbookRepository)
        {
            _sportsbookRepository = sportsbookRepository;
        }

        /// <inheritdoc />
        public async Task<SportsbooksCatalogDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            var catalog = await _sportsbookRepository.GetCatalogAsync(cancellationToken);
            return SportsbooksCatalogDto.FromDomain(catalog);
        }
    }
}
