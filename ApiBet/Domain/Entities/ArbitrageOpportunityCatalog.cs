namespace Domain.Entities
{
    /// <summary>
    /// Instantánea local de oportunidades de arbitraje.
    /// </summary>
    public sealed class ArbitrageOpportunityCatalog
    {
        /// <summary>
        /// Crea el catálogo con sus oportunidades y marcas de tiempo.
        /// </summary>
        public ArbitrageOpportunityCatalog(
            IReadOnlyList<ArbitrageOpportunity> opportunities,
            DateTime? sourceUpdatedAt,
            DateTime? lastSyncedAtUtc = null)
        {
            ArgumentNullException.ThrowIfNull(opportunities);

            Opportunities = opportunities;
            SourceUpdatedAt = sourceUpdatedAt;
            LastSyncedAtUtc = lastSyncedAtUtc;
        }

        /// <summary>
        /// Oportunidades persistidas.
        /// </summary>
        public IReadOnlyList<ArbitrageOpportunity> Opportunities { get; }

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
