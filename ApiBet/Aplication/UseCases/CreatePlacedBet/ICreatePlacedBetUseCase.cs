using Aplication.DTOs;
using Domain.Entities;

namespace Aplication.UseCases.CreatePlacedBet
{
    /// <summary>
    /// Caso de uso para registrar una apuesta ejecutada en dbo.PlacedBets.
    /// </summary>
    public interface ICreatePlacedBetUseCase
    {
        /// <summary>
        /// Registra la apuesta y devuelve la fila creada o existente con su CLV.
        /// </summary>
        Task<PlacedBet> ExecuteAsync(CreatePlacedBetRequestDto request, CancellationToken cancellationToken);
    }
}
