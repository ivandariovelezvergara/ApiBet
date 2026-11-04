using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Abstracción para consultar el catálogo de casas de apuestas en SharpAPI.
    /// </summary>
    public interface ISportsbookCatalogClient
    {
        /// <summary>
        /// Obtiene el catálogo actual de casas de apuestas desde el proveedor externo.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo de casas reportado por SharpAPI.</returns>
        Task<SportsbookCatalog> GetSportsbooksAsync(CancellationToken cancellationToken);
    }
}
