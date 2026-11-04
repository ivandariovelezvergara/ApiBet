using Aplication.DTOs;

namespace Aplication.UseCases.SyncOdds
{
    /// <summary>
    /// Caso de uso que pagina GET /odds con cursor y guarda el resultado en SQL Server.
    /// </summary>
    public interface ISyncOddsUseCase
    {
        /// <summary>
        /// Vacía MarketOdds, recorre las páginas con cursor y persiste cada lote.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Resumen de la sincronización.</returns>
        Task<OddsSyncResultDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
