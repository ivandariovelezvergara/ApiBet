namespace Domain.Entities
{
    /// <summary>
    /// Resultado de persistir un lote delta en MarketOdds.
    /// </summary>
    public sealed class MarketOddsDeltaPersistResult
    {
        /// <summary>
        /// Crea el conteo de filas insertadas o actualizadas y de filas eliminadas.
        /// </summary>
        public MarketOddsDeltaPersistResult(int upsertedCount, int deletedCount)
        {
            UpsertedCount = upsertedCount;
            DeletedCount = deletedCount;
        }

        /// <summary>
        /// Filas insertadas o actualizadas por MERGE.
        /// </summary>
        public int UpsertedCount { get; }

        /// <summary>
        /// Filas eliminadas por el arreglo removed.
        /// </summary>
        public int DeletedCount { get; }
    }
}
