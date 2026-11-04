using Aplication.DTOs;

namespace Aplication.UseCases.GetSports
{
    /// <summary>
    /// Caso de uso que consulta los deportes almacenados en SQL Server.
    /// </summary>
    public interface IGetSportsUseCase
    {
        /// <summary>
        /// Obtiene el catálogo persistido localmente.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo local; vacío si todavía no se sincronizó.</returns>
        Task<SportsCatalogDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
