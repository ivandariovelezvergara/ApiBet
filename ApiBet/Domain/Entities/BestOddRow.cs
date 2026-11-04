namespace Domain.Entities
{
    /// <summary>
    /// Fila de lectura de mejores cuotas según dbo.spr_BestOdds.
    /// </summary>
    public sealed class BestOddRow
    {
        /// <summary>
        /// Identificador del evento.
        /// </summary>
        public string EventId { get; init; } = string.Empty;

        /// <summary>
        /// Nombre visible del evento.
        /// </summary>
        public string EventName { get; init; } = string.Empty;

        /// <summary>
        /// Nombre del deporte según el catálogo.
        /// </summary>
        public string SportName { get; init; } = string.Empty;

        /// <summary>
        /// Nombre visible de la liga.
        /// </summary>
        public string LeagueName { get; init; } = string.Empty;

        /// <summary>
        /// Nombre visible del mercado.
        /// </summary>
        public string MarketName { get; init; } = string.Empty;

        /// <summary>
        /// Selección de la cuota.
        /// </summary>
        public string Selection { get; init; } = string.Empty;

        /// <summary>
        /// Línea; nula si no aplica.
        /// </summary>
        public decimal? Line { get; init; }

        /// <summary>
        /// Indica si es la línea principal.
        /// </summary>
        public bool IsMainLine { get; init; }

        /// <summary>
        /// Mejor cuota americana.
        /// </summary>
        public decimal? BestAmerican { get; init; }

        /// <summary>
        /// Mejor cuota decimal.
        /// </summary>
        public decimal? BestDecimal { get; init; }

        /// <summary>
        /// Probabilidad implícita de la mejor cuota.
        /// </summary>
        public decimal? BestProbability { get; init; }

        /// <summary>
        /// Cuota americana de consenso.
        /// </summary>
        public decimal? ConsensusAmerican { get; init; }

        /// <summary>
        /// Cuota decimal de consenso.
        /// </summary>
        public decimal? ConsensusDecimal { get; init; }

        /// <summary>
        /// Probabilidad implícita de consenso.
        /// </summary>
        public decimal? ConsensusProbability { get; init; }

        /// <summary>
        /// Hold del mercado.
        /// </summary>
        public decimal? MarketHold { get; init; }

        /// <summary>
        /// Nombre visible de la casa con la mejor cuota.
        /// </summary>
        public string BestBookDisplayName { get; init; } = string.Empty;

        /// <summary>
        /// Identificador de la casa en all_books.
        /// </summary>
        public string Book { get; init; } = string.Empty;

        /// <summary>
        /// Nombre visible de la casa en all_books.
        /// </summary>
        public string SportsbookDisplayName { get; init; } = string.Empty;

        /// <summary>
        /// Cuota americana de la casa.
        /// </summary>
        public decimal? BookAmerican { get; init; }

        /// <summary>
        /// Cuota decimal de la casa.
        /// </summary>
        public decimal? BookDecimal { get; init; }

        /// <summary>
        /// Margen de la casa frente al consenso.
        /// </summary>
        public decimal? Edge { get; init; }
    }
}
