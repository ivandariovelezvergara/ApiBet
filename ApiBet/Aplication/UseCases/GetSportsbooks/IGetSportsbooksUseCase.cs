using Aplication.DTOs;

namespace Aplication.UseCases.GetSportsbooks
{
    /// <summary>
    /// Caso de uso que consulta las casas de apuestas almacenadas en SQL Server.
    /// </summary>
    public interface IGetSportsbooksUseCase
    {
        /// <summary>
        /// Obtiene el catálogo persistido localmente.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo local; vacío si todavía no se sincronizó.</returns>
        Task<SportsbooksCatalogDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
