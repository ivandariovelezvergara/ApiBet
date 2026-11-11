using Aplication.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Aplication.UseCases.CreatePlacedBet
{
    /// <summary>
    /// Valida y persiste una apuesta ejecutada en dbo.PlacedBets.
    /// </summary>
    public sealed class CreatePlacedBetUseCase : ICreatePlacedBetUseCase
    {
        private readonly IPlacedBetRepository _placedBetRepository;

        /// <summary>
        /// Inicializa el caso de uso con el repositorio de apuestas.
        /// </summary>
        public CreatePlacedBetUseCase(IPlacedBetRepository placedBetRepository)
        {
            _placedBetRepository = placedBetRepository;
        }

        /// <inheritdoc />
        public async Task<PlacedBet> ExecuteAsync(CreatePlacedBetRequestDto request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrWhiteSpace(request.Partido) ||
                string.IsNullOrWhiteSpace(request.Mercado) ||
                string.IsNullOrWhiteSpace(request.Seleccion) ||
                string.IsNullOrWhiteSpace(request.Casa))
            {
                throw new InvalidOperationException("Partido, mercado, selección y casa son obligatorios.");
            }

            if (request.CuotaTomada <= 1.0m)
            {
                throw new InvalidOperationException("La cuota tomada debe ser mayor a 1.00.");
            }

            if (request.StakeCop <= 0m)
            {
                throw new InvalidOperationException("El monto apostado (Stake) debe ser mayor a 0 COP.");
            }

            return await _placedBetRepository.CreateAsync(request.ToDomain(), cancellationToken);
        }
    }
}
