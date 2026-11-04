using Aplication.DTOs;

namespace Aplication.UseCases.GetArbitrage
{
    /// <summary>
    /// Caso de uso que lee las boletas de arbitraje vía dbo.spr_ArbitrageOpportunities.
    /// </summary>
    public interface IGetArbitrageUseCase
    {
        /// <summary>
        /// Ejecuta el SP de boletas de 2 vías con los filtros de ArbitrageFeed y ValueModel.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Boletas con montos en múltiplos de 100.</returns>
        Task<ArbitrageCatalogDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
