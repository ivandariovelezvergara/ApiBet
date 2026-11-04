using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Contrato de persistencia del catálogo de casas de apuestas en SQL Server.
    /// </summary>
    public interface ISportsbookRepository
    {
        /// <summary>
        /// Reemplaza el catálogo local con la instantánea recibida.
        /// </summary>
        /// <param name="catalog">Catálogo a persistir.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        Task SaveCatalogAsync(SportsbookCatalog catalog, CancellationToken cancellationToken);

        /// <summary>
        /// Recupera el catálogo almacenado localmente.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo persistido; vacío si aún no hay sincronización.</returns>
        Task<SportsbookCatalog> GetCatalogAsync(CancellationToken cancellationToken);
    }
}
