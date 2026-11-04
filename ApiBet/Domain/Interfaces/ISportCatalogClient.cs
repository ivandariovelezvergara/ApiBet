using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Abstracción para consultar el catálogo de deportes en SharpAPI.
    /// </summary>
    public interface ISportCatalogClient
    {
        /// <summary>
        /// Obtiene el catálogo actual de deportes desde el proveedor externo.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo de deportes reportado por SharpAPI.</returns>
        Task<SportCatalog> GetSportsAsync(CancellationToken cancellationToken);
    }
}
