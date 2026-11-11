using Aplication.DTOs;
using Domain.Interfaces;

namespace Aplication.UseCases.GetPlacedBets
{
    /// <summary>
    /// Obtiene la bitácora de apuestas y el resumen de CLV / ROI mediante dbo.spr_PlacedBets_Get.
    /// </summary>
    public sealed class GetPlacedBetsUseCase : IGetPlacedBetsUseCase
    {
        private readonly IPlacedBetRepository _placedBetRepository;

        /// <summary>
        /// Inicializa el caso de uso con el repositorio de apuestas.
        /// </summary>
        public GetPlacedBetsUseCase(IPlacedBetRepository placedBetRepository)
        {
            _placedBetRepository = placedBetRepository;
        }

        /// <inheritdoc />
        public async Task<PlacedBetsCatalogDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            var catalog = await _placedBetRepository.GetCatalogAsync(cancellationToken);
            return PlacedBetsCatalogDto.FromDomain(catalog);
        }
    }
}
