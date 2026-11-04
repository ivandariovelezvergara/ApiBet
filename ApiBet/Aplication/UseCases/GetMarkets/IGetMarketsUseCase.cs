using Aplication.DTOs;

namespace Aplication.UseCases.GetMarkets
{
    /// <summary>
    /// Caso de uso que consulta los mercados almacenados en SQL Server.
    /// </summary>
    public interface IGetMarketsUseCase
    {
        /// <summary>
        /// Obtiene el catálogo persistido localmente.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo local; vacío si todavía no se sincronizó.</returns>
        Task<MarketsCatalogDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
