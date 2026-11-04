namespace Domain.Entities
{
    /// <summary>
    /// Mejor cuota de una selección según el endpoint odds/best de SharpAPI.
    /// </summary>
    public class BestOdd
    {
        /// <summary>
        /// Constructor requerido por el mapeo de persistencia.
        /// </summary>
        protected BestOdd()
        {
        }

        /// <summary>
        /// Crea una fila de mejores cuotas a partir del catálogo remoto.
        /// </summary>
        public BestOdd(
            string eventId,
            string eventName,
            string sport,
            string league,
            string marketType,
            string selection,
            decimal? line,
            bool isMainLine,
            decimal? bestAmerican,
            decimal? bestDecimal,
            decimal? bestProbability,
            decimal? consensusAmerican,
            decimal? consensusDecimal,
            decimal? consensusProbability,
            decimal? marketHold,
            string? bestBook,
            DateTime? timestamp,
            string? playerName,
            string? statCategory,
            IEnumerable<BestOddBook> books)
        {
            ArgumentNullException.ThrowIfNull(books);

            Id = Guid.NewGuid().ToString("N");
            EventId = eventId ?? string.Empty;
            EventName = eventName ?? string.Empty;
            Sport = sport ?? string.Empty;
            League = league ?? string.Empty;
            MarketType = marketType ?? string.Empty;
            Selection = selection ?? string.Empty;
            Line = line;
            IsMainLine = isMainLine;
            BestAmerican = bestAmerican;
            BestDecimal = bestDecimal;
            BestProbability = bestProbability;
            ConsensusAmerican = consensusAmerican;
            ConsensusDecimal = consensusDecimal;
            ConsensusProbability = consensusProbability;
            MarketHold = marketHold;
            BestBook = bestBook;
            Timestamp = timestamp;
            PlayerName = playerName;
            StatCategory = statCategory;
            CreatedAtUtc = DateTime.Now;
            Books = books.ToList();
            foreach (var book in Books)
            {
                book.AssignBestOdd(Id);
            }
        }

        /// <summary>
        /// Identificador único de la fila recibida desde SharpAPI.
        /// </summary>
        public string Id { get; protected set; } = string.Empty;

        /// <summary>
        /// Identificador del evento.
        /// </summary>
        public string EventId { get; protected set; } = string.Empty;

        /// <summary>
        /// Nombre visible del evento.
        /// </summary>
        public string EventName { get; protected set; } = string.Empty;

        /// <summary>
        /// Slug del deporte.
        /// </summary>
        public string Sport { get; protected set; } = string.Empty;

        /// <summary>
        /// Slug de la liga.
        /// </summary>
        public string League { get; protected set; } = string.Empty;

        /// <summary>
        /// Tipo de mercado (moneyline, spread, total, etc.).
        /// </summary>
        public string MarketType { get; protected set; } = string.Empty;

        /// <summary>
        /// Nombre de la selección.
        /// </summary>
        public string Selection { get; protected set; } = string.Empty;

        /// <summary>
        /// Línea asociada (spread o total); nula en mercados sin línea.
        /// </summary>
        public decimal? Line { get; protected set; }

        /// <summary>
        /// Indica si es la línea principal del cohort.
        /// </summary>
        public bool IsMainLine { get; protected set; }

        /// <summary>
        /// Mejor cuota americana entre casas.
        /// </summary>
        public decimal? BestAmerican { get; protected set; }

        /// <summary>
        /// Mejor cuota decimal entre casas.
        /// </summary>
        public decimal? BestDecimal { get; protected set; }

        /// <summary>
        /// Probabilidad implícita de la mejor cuota.
        /// </summary>
        public decimal? BestProbability { get; protected set; }

        /// <summary>
        /// Cuota americana de consenso.
        /// </summary>
        public decimal? ConsensusAmerican { get; protected set; }

        /// <summary>
        /// Cuota decimal de consenso.
        /// </summary>
        public decimal? ConsensusDecimal { get; protected set; }

        /// <summary>
        /// Probabilidad implícita de consenso.
        /// </summary>
        public decimal? ConsensusProbability { get; protected set; }

        /// <summary>
        /// Hold del mercado en porcentaje.
        /// </summary>
        public decimal? MarketHold { get; protected set; }

        /// <summary>
        /// Casa con la mejor cuota.
        /// </summary>
        public string? BestBook { get; protected set; }

        /// <summary>
        /// Marca de tiempo de frescura informada por SharpAPI.
        /// </summary>
        public DateTime? Timestamp { get; protected set; }

        /// <summary>
        /// Nombre del jugador en mercados de props.
        /// </summary>
        public string? PlayerName { get; protected set; }

        /// <summary>
        /// Categoría estadística en mercados de props.
        /// </summary>
        public string? StatCategory { get; protected set; }

        /// <summary>
        /// Fecha y hora UTC exactas de inserción local.
        /// </summary>
        public DateTime CreatedAtUtc { get; protected set; }

        /// <summary>
        /// Cuotas de todas las casas comparadas.
        /// </summary>
        public ICollection<BestOddBook> Books { get; protected set; } = new List<BestOddBook>();

    }
}
