using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Contrato de persistencia de las cuotas de mercado en SQL Server.
    /// </summary>
    public interface IOddsRepository
    {
        /// <summary>
        /// Vacía la tabla MarketOdds antes de recargar el catálogo.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        Task ClearAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Vacía la tabla MarketOddsDelta antes de recargar GET /odds/delta.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        Task ClearDeltaAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Inserta una página de cuotas mediante dbo.spr_MarketOdds_BulkLoad.
        /// </summary>
        /// <param name="odds">Filas de la iteración actual.</param>
        /// <param name="sourceUpdatedAt">Marca de tiempo del origen.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Cantidad de filas persistidas en la página.</returns>
        Task<int> SavePageAsync(
            IReadOnlyList<MarketOdd> odds,
            DateTime? sourceUpdatedAt,
            CancellationToken cancellationToken);

        /// <summary>
        /// Aplica un lote incremental mediante dbo.spr_MarketOddsDelta_DeltaLoad (MERGE + DELETE sobre MarketOddsDelta).
        /// </summary>
        /// <param name="odds">Cuotas activas o actualizadas de la iteración actual.</param>
        /// <param name="removed">Identificadores que deben eliminarse de MarketOddsDelta.</param>
        /// <param name="sourceUpdatedAt">Marca de tiempo del origen.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Conteo de filas insertadas o actualizadas y de filas eliminadas.</returns>
        Task<MarketOddsDeltaPersistResult> SaveDeltaAsync(
            IReadOnlyList<MarketOdd> odds,
            IReadOnlyList<RemovedOdd> removed,
            DateTime? sourceUpdatedAt,
            CancellationToken cancellationToken);
    }
}
