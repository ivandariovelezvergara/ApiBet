namespace Aplication.DTOs
{
    /// <summary>
    /// Resultado de una sincronización incremental de cuotas GET /odds/delta.
    /// </summary>
    public sealed class OddsDeltaSyncResultDto
    {
        /// <summary>
        /// Filas insertadas o actualizadas al finalizar el ciclo.
        /// </summary>
        public int UpsertedCount { get; init; }

        /// <summary>
        /// Filas eliminadas a partir del arreglo removed.
        /// </summary>
        public int DeletedCount { get; init; }

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
        /// Marca since enviada a SharpAPI: inicio del día UTC actual.
        /// </summary>
        public required string Since { get; init; }

        /// <summary>
        /// Tamaño de página (OddsDelta:Limit).
        /// </summary>
        public int Limit { get; init; }

        /// <summary>
        /// Offset inicial (OddsDelta:Offset).
        /// </summary>
        public int Offset { get; init; }

        /// <summary>
        /// Máximo de páginas (OddsDelta:MaxPages).
        /// </summary>
        public int MaxPages { get; init; }

        /// <summary>
        /// Espera entre páginas en segundos (OddsDelta:DelaySeconds).
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
        /// Indica si alguna página llegó con overflow.
        /// </summary>
        public bool Overflow { get; init; }

        /// <summary>
        /// Indica si alguna página llegó con removed truncado.
        /// </summary>
        public bool RemovedTruncated { get; init; }

        /// <summary>
        /// Indica si SharpAPI ajustó el parámetro since.
        /// </summary>
        public bool SinceClamped { get; init; }

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
