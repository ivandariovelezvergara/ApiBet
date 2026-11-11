using Domain.Entities;

namespace Aplication.UseCases.UpdatePlacedBetStatus
{
    /// <summary>
    /// Caso de uso para liquidar o actualizar el estado de una apuesta en dbo.PlacedBets.
    /// </summary>
    public interface IUpdatePlacedBetStatusUseCase
    {
        /// <summary>
        /// Actualiza el estado (WON, LOST, PUSH, VOID, PENDING) y devuelve la apuesta actualizada.
        /// </summary>
        Task<PlacedBet?> ExecuteAsync(int id, string status, CancellationToken cancellationToken);
    }
}
