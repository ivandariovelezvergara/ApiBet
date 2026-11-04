namespace Aplication.DTOs
{
    /// <summary>
    /// Resultado de una sincronización paginada de oportunidades de arbitraje.
    /// </summary>
    public sealed class ArbitrageSyncResultDto
    {
        /// <summary>
        /// Filas de ArbitrageOpportunities persistidas al finalizar el ciclo.
        /// </summary>
        public int StoredCount { get; init; }

        /// <summary>
        /// Filas de ArbitrageOpportunityLegs persistidas al finalizar el ciclo.
        /// </summary>
        public int StoredLegCount { get; init; }

        /// <summary>
        /// Oportunidades borradas por haber cumplido ArbitrageFeed:RetentionMinutes.
        /// </summary>
        public int DeletedCount { get; init; }

        /// <summary>
        /// Páginas solicitadas a SharpAPI.
        /// </summary>
        public int PagesFetched { get; init; }

        /// <summary>
        /// Último offset procesado.
        /// </summary>
        public int LastOffset { get; init; }

        /// <summary>
        /// Tamaño de página enviado a SharpAPI (ArbitrageFeed:Limit).
        /// </summary>
        public int Limit { get; init; }

        /// <summary>
        /// Offset inicial (ArbitrageFeed:Offset).
        /// </summary>
        public int Offset { get; init; }

        /// <summary>
        /// Máximo de páginas (ArbitrageFeed:MaxPages).
        /// </summary>
        public int MaxPages { get; init; }

        /// <summary>
        /// Espera entre páginas en segundos (ArbitrageFeed:DelaySeconds).
        /// </summary>
        public int DelaySeconds { get; init; }

        /// <summary>
        /// Minutos de retención aplicados al limpiar (ArbitrageFeed:RetentionMinutes).
        /// </summary>
        public int RetentionMinutes { get; init; }

        /// <summary>
        /// Deportes sincronizados; vacío si se pidieron todos.
        /// </summary>
        public required IReadOnlyList<string> Sports { get; init; }

        /// <summary>
        /// Filtro de liga enviado a SharpAPI; nulo si no se envió.
        /// </summary>
        public string? League { get; init; }

        /// <summary>
        /// Casas enviadas a SharpAPI y al SP de carga (ArbitrageFeed:Sportsbook).
        /// </summary>
        public string? Sportsbook { get; init; }

        /// <summary>
        /// Filtro de mercado enviado a SharpAPI; nulo si no se envió.
        /// </summary>
        public string? Market { get; init; }

        /// <summary>
        /// Beneficio mínimo enviado a SharpAPI.
        /// </summary>
        public decimal? MinProfit { get; init; }

        /// <summary>
        /// Filtro is_live; nulo = prepartido y en vivo.
        /// </summary>
        public bool? IsLive { get; init; }

        /// <summary>
        /// Código de estado para deep links (ArbitrageFeed:State).
        /// </summary>
        public string? State { get; init; }

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
