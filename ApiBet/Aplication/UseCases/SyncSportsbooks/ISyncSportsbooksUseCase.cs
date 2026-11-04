using Aplication.DTOs;

namespace Aplication.UseCases.SyncSportsbooks
{
    /// <summary>
    /// Caso de uso que descarga el catálogo de casas de SharpAPI y lo guarda en SQL Server.
    /// </summary>
    public interface ISyncSportsbooksUseCase
    {
        /// <summary>
        /// Ejecuta la sincronización remota y la persistencia local.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo persistido tras la sincronización.</returns>
        Task<SportsbooksCatalogDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
