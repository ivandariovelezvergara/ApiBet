using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Contrato de persistencia para la bitácora de apuestas ejecutadas (dbo.PlacedBets).
    /// </summary>
    public interface IPlacedBetRepository
    {
        /// <summary>
        /// Registra una nueva apuesta ejecutada mediante dbo.spr_PlacedBets_Insert.
        /// </summary>
        Task<PlacedBet> CreateAsync(NewPlacedBet bet, CancellationToken cancellationToken);

        /// <summary>
        /// Actualiza el estado (WON, LOST, PUSH, VOID, PENDING) mediante dbo.spr_PlacedBets_UpdateStatus.
        /// </summary>
        Task<PlacedBet?> UpdateStatusAsync(int id, string status, CancellationToken cancellationToken);

        /// <summary>
        /// Obtiene la bitácora de apuestas y el resumen de CLV / ROI mediante dbo.spr_PlacedBets_Get.
        /// </summary>
        Task<PlacedBetsCatalog> GetCatalogAsync(CancellationToken cancellationToken);
    }
}
