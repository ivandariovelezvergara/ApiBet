namespace Domain.Entities
{
    /// <summary>
    /// Página de cuotas de mercado devuelta por GET /odds de SharpAPI.
    /// </summary>
    public sealed class MarketOddsPage
    {
        /// <summary>
        /// Crea la página con sus filas y metadatos de paginación.
        /// </summary>
        public MarketOddsPage(
            IReadOnlyList<MarketOdd> items,
            bool hasMore,
            int? nextOffset,
            string? nextCursor,
            DateTime? sourceUpdatedAt)
        {
            ArgumentNullException.ThrowIfNull(items);

            Items = items;
            HasMore = hasMore;
            NextOffset = nextOffset;
            NextCursor = nextCursor;
            SourceUpdatedAt = sourceUpdatedAt;
        }

        /// <summary>
        /// Filas de la página actual.
        /// </summary>
        public IReadOnlyList<MarketOdd> Items { get; }

        /// <summary>
        /// Indica si existen más resultados más allá de esta página.
        /// </summary>
        public bool HasMore { get; }

        /// <summary>
        /// Siguiente offset sugerido por SharpAPI.
        /// </summary>
        public int? NextOffset { get; }

        /// <summary>
        /// Cursor opaco de paginación profunda (next_cursor).
        /// </summary>
        public string? NextCursor { get; }

        /// <summary>
        /// Marca de tiempo de actualización del origen.
        /// </summary>
        public DateTime? SourceUpdatedAt { get; }
    }
}
