namespace Aplication.DTOs
{
    /// <summary>
    /// Resultado de una sincronización paginada de cuotas GET /odds.
    /// </summary>
    public sealed class OddsSyncResultDto
    {
        /// <summary>
        /// Filas de MarketOdds persistidas al finalizar el ciclo.
        /// </summary>
        public int StoredCount { get; init; }

        /// <summary>
        /// Páginas solicitadas a SharpAPI.
        /// </summary>
        public int PagesFetched { get; init; }

        /// <summary>
        /// Último offset informado por SharpAPI.
        /// </summary>
        public int? LastOffset { get; init; }

        /// <summary>
        /// Último cursor opaco (next_cursor) usado o recibido.
        /// </summary>
        public string? LastCursor { get; init; }

        /// <summary>
        /// Tamaño de página (OddsFeed:Limit).
        /// </summary>
        public int Limit { get; init; }

        /// <summary>
        /// Offset inicial (OddsFeed:Offset).
        /// </summary>
        public int Offset { get; init; }

        /// <summary>
        /// Máximo de páginas (OddsFeed:MaxPages).
        /// </summary>
        public int MaxPages { get; init; }

        /// <summary>
        /// Espera entre páginas en segundos (OddsFeed:DelaySeconds).
        /// </summary>
        public int DelaySeconds { get; init; }

        /// <summary>
        /// Deportes sincronizados; vacío si se pidieron todos.
        /// </summary>
        public required IReadOnlyList<string> Sports { get; init; }

        /// <summary>
        /// Mercados enviados a SharpAPI.
        /// </summary>
        public string? Market { get; init; }

        /// <summary>
        /// Filtro is_live enviado a SharpAPI.
        /// </summary>
        public bool? IsLive { get; init; }

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
