using Aplication.DTOs;

namespace Aplication.UseCases.GetPlacedBets
{
    /// <summary>
    /// Caso de uso para consultar la bitácora de apuestas y sus métricas de CLV y ROI.
    /// </summary>
    public interface IGetPlacedBetsUseCase
    {
        /// <summary>
        /// Devuelve la lista de apuestas registradas y el resumen ejecutivo.
        /// </summary>
        Task<PlacedBetsCatalogDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
