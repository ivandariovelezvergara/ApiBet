namespace Domain.Entities
{
    /// <summary>
    /// Página de cuotas incrementales devuelta por GET /odds/delta de SharpAPI.
    /// </summary>
    public sealed class MarketOddsDeltaPage
    {
        /// <summary>
        /// Crea la página con altas/actualizaciones, retiros y metadatos de paginación.
        /// </summary>
        public MarketOddsDeltaPage(
            IReadOnlyList<MarketOdd> items,
            IReadOnlyList<RemovedOdd> removed,
            bool hasMore,
            int? nextOffset,
            string? nextCursor,
            DateTime? sourceUpdatedAt,
            bool overflow,
            bool removedTruncated,
            bool sinceClamped)
        {
            ArgumentNullException.ThrowIfNull(items);
            ArgumentNullException.ThrowIfNull(removed);

            Items = items;
            Removed = removed;
            HasMore = hasMore;
            NextOffset = nextOffset;
            NextCursor = nextCursor;
            SourceUpdatedAt = sourceUpdatedAt;
            Overflow = overflow;
            RemovedTruncated = removedTruncated;
            SinceClamped = sinceClamped;
        }

        /// <summary>
        /// Cuotas activas o actualizadas de la página actual.
        /// </summary>
        public IReadOnlyList<MarketOdd> Items { get; }

        /// <summary>
        /// Identificadores que deben eliminarse de MarketOddsDelta.
        /// </summary>
        public IReadOnlyList<RemovedOdd> Removed { get; }

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

        /// <summary>
        /// Indica si SharpAPI recortó el lote por desbordamiento.
        /// </summary>
        public bool Overflow { get; }

        /// <summary>
        /// Indica si la lista de retirados llegó truncada.
        /// </summary>
        public bool RemovedTruncated { get; }

        /// <summary>
        /// Indica si SharpAPI ajustó el parámetro since.
        /// </summary>
        public bool SinceClamped { get; }
    }
}
