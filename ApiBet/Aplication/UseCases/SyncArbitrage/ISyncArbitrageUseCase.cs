using Aplication.DTOs;

namespace Aplication.UseCases.SyncArbitrage
{
    /// <summary>
    /// Caso de uso que pagina opportunities/arbitrage y guarda el resultado en SQL Server.
    /// </summary>
    public interface ISyncArbitrageUseCase
    {
        /// <summary>
        /// Recorre las páginas, persiste cada lote y borra filas que ya cumplieron la retención.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Resumen de la sincronización.</returns>
        Task<ArbitrageSyncResultDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
