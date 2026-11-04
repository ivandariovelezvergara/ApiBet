namespace Aplication.DTOs
{
    /// <summary>
    /// Resultado de una sincronización paginada de mejores cuotas.
    /// </summary>
    public sealed class BestOddsSyncResultDto
    {
        /// <summary>
        /// Filas de BestOdds persistidas al finalizar el ciclo.
        /// </summary>
        public int StoredCount { get; init; }

        /// <summary>
        /// Filas de BestOddBooks (all_books) persistidas al finalizar el ciclo.
        /// </summary>
        public int StoredBookCount { get; init; }

        /// <summary>
        /// Páginas solicitadas a SharpAPI.
        /// </summary>
        public int PagesFetched { get; init; }

        /// <summary>
        /// Último offset procesado.
        /// </summary>
        public int LastOffset { get; init; }

        /// <summary>
        /// Tamaño de página (OddsSync:Limit).
        /// </summary>
        public int Limit { get; init; }

        /// <summary>
        /// Offset inicial (OddsSync:Offset).
        /// </summary>
        public int Offset { get; init; }

        /// <summary>
        /// Offset máximo (OddsSync:MaxOffset).
        /// </summary>
        public int MaxOffset { get; init; }

        /// <summary>
        /// Incremento de offset (OddsSync:OffsetIncrement).
        /// </summary>
        public int OffsetIncrement { get; init; }

        /// <summary>
        /// Espera entre páginas en segundos (OddsSync:DelaySeconds).
        /// </summary>
        public int DelaySeconds { get; init; }

        /// <summary>
        /// Deportes sincronizados.
        /// </summary>
        public required IReadOnlyList<string> Sports { get; init; }

        /// <summary>
        /// Filtro de mercado enviado a SharpAPI; nulo si no se envió.
        /// </summary>
        public string? Market { get; init; }

        /// <summary>
        /// Filtro is_main_line enviado a SharpAPI.
        /// </summary>
        public bool? IsMainLine { get; init; }

        /// <summary>
        /// Query relativa de la primera página enviada a SharpAPI.
        /// </summary>
        public required string SharpApiPath { get; init; }

        /// <summary>
        /// Marca de tiempo del origen.
        /// </summary>
        public DateTime? UpdatedAt { get; init; }

        /// <summary>
        /// Marca de tiempo UTC de la persistencia local.
        /// </summary>
        public DateTime LastSyncedAtUtc { get; init; }

        /// <summary>
        /// Segundos que tomó la sincronización completa.
        /// </summary>
        public double DurationSeconds { get; init; }
    }
}
