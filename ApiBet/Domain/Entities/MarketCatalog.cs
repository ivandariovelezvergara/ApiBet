namespace Domain.Entities
{
    /// <summary>
    /// Instantánea del catálogo de mercados, remota o persistida localmente.
    /// </summary>
    public sealed class MarketCatalog
    {
        /// <summary>
        /// Crea el catálogo con sus mercados y marcas de tiempo.
        /// </summary>
        /// <param name="markets">Mercados incluidos en la instantánea.</param>
        /// <param name="sourceUpdatedAt">Momento en que SharpAPI actualizó el catálogo.</param>
        /// <param name="lastSyncedAtUtc">Momento local en que se guardó en SQL Server, si aplica.</param>
        public MarketCatalog(
            IReadOnlyList<Market> markets,
            DateTime? sourceUpdatedAt,
            DateTime? lastSyncedAtUtc = null)
        {
            ArgumentNullException.ThrowIfNull(markets);

            Markets = markets;
            SourceUpdatedAt = sourceUpdatedAt;
            LastSyncedAtUtc = lastSyncedAtUtc;
        }

        /// <summary>
        /// Mercados incluidos en el catálogo.
        /// </summary>
        public IReadOnlyList<Market> Markets { get; }

        /// <summary>
        /// Fecha y hora en que SharpAPI actualizó el origen.
        /// </summary>
        public DateTime? SourceUpdatedAt { get; }

        /// <summary>
        /// Fecha y hora UTC de la última sincronización local.
        /// </summary>
        public DateTime? LastSyncedAtUtc { get; }
    }
}
