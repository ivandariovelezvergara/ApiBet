using Domain.Entities;
using Domain.Interfaces;

namespace Aplication.UseCases.UpdatePlacedBetStatus
{
    /// <summary>
    /// Liquida una apuesta registrada en dbo.PlacedBets.
    /// </summary>
    public sealed class UpdatePlacedBetStatusUseCase : IUpdatePlacedBetStatusUseCase
    {
        private static readonly HashSet<string> ValidStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "PENDING", "WON", "LOST", "PUSH", "VOID"
        };

        private readonly IPlacedBetRepository _placedBetRepository;

        /// <summary>
        /// Inicializa el caso de uso con el repositorio de apuestas.
        /// </summary>
        public UpdatePlacedBetStatusUseCase(IPlacedBetRepository placedBetRepository)
        {
            _placedBetRepository = placedBetRepository;
        }

        /// <inheritdoc />
        public async Task<PlacedBet?> ExecuteAsync(int id, string status, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                throw new InvalidOperationException("El identificador de la apuesta no es válido.");
            }

            if (string.IsNullOrWhiteSpace(status) || !ValidStatuses.Contains(status.Trim()))
            {
                throw new InvalidOperationException("El estado debe ser PENDING, WON, LOST, PUSH o VOID.");
            }

            return await _placedBetRepository.UpdateStatusAsync(id, status.Trim().ToUpperInvariant(), cancellationToken);
        }
    }
}
