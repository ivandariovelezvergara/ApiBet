using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Abstracción para consultar oportunidades de arbitraje en SharpAPI.
    /// </summary>
    public interface IArbitrageClient
    {
        /// <summary>
        /// Obtiene una página de oportunidades GET /opportunities/arbitrage.
        /// </summary>
        /// <param name="limit">Tamaño de página (máximo 200).</param>
        /// <param name="offset">Desplazamiento de paginación.</param>
        /// <param name="sports">Slugs de deporte separados por coma; nulo para omitir.</param>
        /// <param name="league">Slugs de liga separados por coma; nulo para omitir.</param>
        /// <param name="sportsbook">Casas separadas por coma; nulo para omitir.</param>
        /// <param name="market">Tipos de mercado separados por coma; nulo para omitir.</param>
        /// <param name="minProfit">Beneficio mínimo en porcentaje; nulo para omitir.</param>
        /// <param name="isLive">Filtro is_live; nulo para prepartido y en vivo.</param>
        /// <param name="state">Código de estado EE. UU. para deep links; nulo para el default de SharpAPI.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Página de oportunidades.</returns>
        Task<ArbitrageOpportunitiesPage> GetArbitrageAsync(
            int limit,
            int offset,
            string? sports,
            string? league,
            string? sportsbook,
            string? market,
            decimal? minProfit,
            bool? isLive,
            string? state,
            CancellationToken cancellationToken);
    }
}
