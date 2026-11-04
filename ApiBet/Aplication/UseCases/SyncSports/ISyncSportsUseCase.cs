using Aplication.DTOs;

namespace Aplication.UseCases.SyncSports
{
    /// <summary>
    /// Caso de uso que descarga el catálogo de SharpAPI y lo guarda en SQL Server.
    /// </summary>
    public interface ISyncSportsUseCase
    {
        /// <summary>
        /// Ejecuta la sincronización remota y la persistencia local.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo persistido tras la sincronización.</returns>
        Task<SportsCatalogDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
