using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Contrato de persistencia de las oportunidades de arbitraje en SQL Server.
    /// </summary>
    public interface IArbitrageOpportunityRepository
    {
        /// <summary>
        /// Elimina oportunidades cuya inserción local ya cumplió el tiempo de retención.
        /// Las patas se borran por CASCADE. Las filas más jóvenes se conservan.
        /// </summary>
        /// <param name="retentionMinutes">Minutos de vida mínima para poder borrar.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Cantidad de oportunidades eliminadas.</returns>
        Task<int> ClearExpiredAsync(int retentionMinutes, CancellationToken cancellationToken);

        /// <summary>
        /// Inserta una página mediante dbo.spr_ArbitrageOpportunities_BulkLoad.
        /// </summary>
        /// <param name="opportunities">Filas de la iteración actual.</param>
        /// <param name="sourceUpdatedAt">Marca de tiempo del origen.</param>
        /// <param name="sportsbooks">Casas permitidas, separadas por coma; nulo o vacío para no filtrar.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Cantidad de oportunidades y patas persistidas en la página.</returns>
        Task<(int OpportunityCount, int LegCount)> SavePageAsync(
            IReadOnlyList<ArbitrageOpportunity> opportunities,
            DateTime? sourceUpdatedAt,
            string? sportsbooks,
            CancellationToken cancellationToken);

        /// <summary>
        /// Recupera las boletas de 2 vías mediante dbo.spr_ArbitrageOpportunities.
        /// </summary>
        /// <param name="bankroll">Banca total en COP.</param>
        /// <param name="sport">Slug de deporte; nulo para devolver todas.</param>
        /// <param name="minProfitPercent">Beneficio mínimo en porcentaje.</param>
        /// <param name="isLive">Filtro en vivo; nulo para ambos.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Boletas con montos en múltiplos de 100.</returns>
        Task<IReadOnlyList<ArbitrageBetRow>> GetBetsAsync(
            decimal bankroll,
            string? sport,
            decimal minProfitPercent,
            bool? isLive,
            CancellationToken cancellationToken);
    }
}
