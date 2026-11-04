using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Contrato de persistencia del catálogo de mercados en SQL Server.
    /// </summary>
    public interface IMarketRepository
    {
        /// <summary>
        /// Reemplaza el catálogo local con la instantánea recibida.
        /// </summary>
        /// <param name="catalog">Catálogo a persistir.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        Task SaveCatalogAsync(MarketCatalog catalog, CancellationToken cancellationToken);

        /// <summary>
        /// Recupera el catálogo almacenado localmente.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo persistido; vacío si aún no hay sincronización.</returns>
        Task<MarketCatalog> GetCatalogAsync(CancellationToken cancellationToken);
    }
}
