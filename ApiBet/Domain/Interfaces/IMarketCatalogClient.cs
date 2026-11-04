using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Abstracción para consultar el catálogo de mercados en SharpAPI.
    /// </summary>
    public interface IMarketCatalogClient
    {
        /// <summary>
        /// Obtiene el catálogo actual de mercados desde el proveedor externo.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo de mercados reportado por SharpAPI.</returns>
        Task<MarketCatalog> GetMarketsAsync(CancellationToken cancellationToken);
    }
}
