namespace Domain.Entities
{
    /// <summary>
    /// Página de mejores cuotas devuelta por SharpAPI.
    /// </summary>
    public sealed class BestOddsPage
    {
        /// <summary>
        /// Crea la página con sus filas y metadatos de paginación.
        /// </summary>
        public BestOddsPage(
            IReadOnlyList<BestOdd> items,
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
        /// Filas de la página actual.
        /// </summary>
        public IReadOnlyList<BestOdd> Items { get; }

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
