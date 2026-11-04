using Aplication.DTOs;

namespace Aplication.UseCases.SyncMarkets
{
    /// <summary>
    /// Caso de uso que descarga el catálogo de mercados de SharpAPI y lo guarda en SQL Server.
    /// </summary>
    public interface ISyncMarketsUseCase
    {
        /// <summary>
        /// Ejecuta la sincronización remota y la persistencia local.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo persistido tras la sincronización.</returns>
        Task<MarketsCatalogDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
