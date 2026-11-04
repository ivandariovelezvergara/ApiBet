using Aplication.DTOs;
using Domain.Interfaces;

namespace Aplication.UseCases.GetSports
{
    /// <summary>
    /// Recupera el catálogo de deportes desde la base SQL Server local.
    /// </summary>
    public sealed class GetSportsUseCase : IGetSportsUseCase
    {
        private readonly ISportRepository _sportRepository;

        /// <summary>
        /// Inicializa el caso de uso con el repositorio de deportes.
        /// </summary>
        /// <param name="sportRepository">Repositorio local de deportes.</param>
        public GetSportsUseCase(ISportRepository sportRepository)
        {
            _sportRepository = sportRepository;
        }

        /// <inheritdoc />
        public async Task<SportsCatalogDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            var catalog = await _sportRepository.GetCatalogAsync(cancellationToken);
            return SportsCatalogDto.FromDomain(catalog);
        }
    }
}
