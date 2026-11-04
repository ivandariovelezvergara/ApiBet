namespace Domain.Entities
{
    /// <summary>
    /// Página de oportunidades de arbitraje devuelta por SharpAPI.
    /// </summary>
    public sealed class ArbitrageOpportunitiesPage
    {
        /// <summary>
        /// Crea la página con sus filas y metadatos de paginación.
        /// </summary>
        public ArbitrageOpportunitiesPage(
            IReadOnlyList<ArbitrageOpportunity> items,
            bool hasMore,
            int? nextOffset,
            DateTime? sourceUpdatedAt)
        {
            ArgumentNullException.ThrowIfNull(items);

            Items = items;
            HasMore = hasMore;
            NextOffset = nextOffset;
            SourceUpdatedAt = sourceUpdatedAt;
        }

        /// <summary>
        /// Oportunidades de la página actual.
        /// </summary>
        public IReadOnlyList<ArbitrageOpportunity> Items { get; }

        /// <summary>
        /// Indica si existen más resultados más allá de esta página.
        /// </summary>
        public bool HasMore { get; }

        /// <summary>
        /// Siguiente offset sugerido por SharpAPI.
        /// </summary>
        public int? NextOffset { get; }

        /// <summary>
        /// Marca de tiempo de actualización del origen.
        /// </summary>
        public DateTime? SourceUpdatedAt { get; }
    }
}
