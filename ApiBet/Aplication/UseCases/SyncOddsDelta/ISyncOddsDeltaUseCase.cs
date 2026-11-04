using Aplication.DTOs;

namespace Aplication.UseCases.SyncOddsDelta
{
    /// <summary>
    /// Caso de uso que pagina GET /odds/delta y aplica altas, actualizaciones y retiros en SQL Server.
    /// </summary>
    public interface ISyncOddsDeltaUseCase
    {
        /// <summary>
        /// Vacía MarketOddsDelta, recorre las páginas delta con cursor y persiste cada lote.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Resumen de la sincronización incremental.</returns>
        Task<OddsDeltaSyncResultDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
