using Aplication.DTOs;

namespace Aplication.UseCases.SyncBestOdds
{
    /// <summary>
    /// Caso de uso que pagina odds/best y guarda el resultado en SQL Server.
    /// </summary>
    public interface ISyncBestOddsUseCase
    {
        /// <summary>
        /// Ejecuta el ciclo de offset configurado y persiste las cuotas.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Resumen de la sincronización.</returns>
        Task<BestOddsSyncResultDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
