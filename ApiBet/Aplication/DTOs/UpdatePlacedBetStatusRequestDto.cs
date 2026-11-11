namespace Aplication.DTOs
{
    /// <summary>
    /// Solicitud para liquidar o cambiar el estado de una apuesta en la bitácora.
    /// </summary>
    public sealed class UpdatePlacedBetStatusRequestDto
    {
        /// <summary>Nuevo estado: PENDING, WON, LOST, PUSH o VOID.</summary>
        public required string Estado { get; init; }
    }
}
