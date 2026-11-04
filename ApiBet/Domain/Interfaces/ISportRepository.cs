using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Contrato de persistencia del catálogo de deportes en SQL Server.
    /// </summary>
    public interface ISportRepository
    {
        /// <summary>
        /// Reemplaza el catálogo local con la instantánea recibida.
        /// </summary>
        /// <param name="catalog">Catálogo a persistir.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        Task SaveCatalogAsync(SportCatalog catalog, CancellationToken cancellationToken);

        /// <summary>
        /// Recupera el catálogo almacenado localmente.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo persistido; vacío si aún no hay sincronización.</returns>
        Task<SportCatalog> GetCatalogAsync(CancellationToken cancellationToken);
    }
}
