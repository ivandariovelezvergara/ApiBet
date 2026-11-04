using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Abstracción para consultar el catálogo de ligas en SharpAPI.
    /// </summary>
    public interface ILeagueCatalogClient
    {
        /// <summary>
        /// Obtiene el catálogo actual de ligas desde el proveedor externo.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo de ligas reportado por SharpAPI.</returns>
        Task<LeagueCatalog> GetLeaguesAsync(CancellationToken cancellationToken);
    }
}
