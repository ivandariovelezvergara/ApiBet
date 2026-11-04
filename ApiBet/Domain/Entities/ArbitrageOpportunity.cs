namespace Domain.Entities
{
    /// <summary>
    /// Oportunidad de arbitraje del endpoint GET /opportunities/arbitrage de SharpAPI.
    /// </summary>
    public class ArbitrageOpportunity
    {
        /// <summary>
        /// Constructor requerido por el mapeo de persistencia.
        /// </summary>
        protected ArbitrageOpportunity()
        {
        }

        /// <summary>
        /// Crea una oportunidad de arbitraje a partir del catálogo remoto.
        /// </summary>
        public ArbitrageOpportunity(
            string? id,
            DateTime? detectedAt,
            decimal? estimatedNetProfitPercent,
            bool evAvailable,
            decimal? evPercentage,
            string eventId,
            string eventName,
            decimal? impliedTotal,
            bool isAlternateLine,
            bool isLive,
            bool isPlayerProp,
            string league,
            string? leagueLabel,
            decimal? line,
            bool lineUnchanged,
            string? marketLabel,
            string marketType,
            decimal? oldestOddsAgeSeconds,
            string? playerName,
            bool possiblyStale,
            decimal? profitPercent,
            string sport,
            DateTime? startTime,
            string? statCategory,
            string? warnings,
            IEnumerable<ArbitrageOpportunityLeg> legs)
        {
            ArgumentNullException.ThrowIfNull(legs);

            Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString("N") : id.Trim();
            DetectedAt = detectedAt;
            EstimatedNetProfitPercent = estimatedNetProfitPercent;
            EvAvailable = evAvailable;
            EvPercentage = evPercentage;
            EventId = eventId ?? string.Empty;
            EventName = eventName ?? string.Empty;
            ImpliedTotal = impliedTotal;
            IsAlternateLine = isAlternateLine;
            IsLive = isLive;
            IsPlayerProp = isPlayerProp;
            League = league ?? string.Empty;
            LeagueLabel = leagueLabel;
            Line = line;
            LineUnchanged = lineUnchanged;
            MarketLabel = marketLabel;
            MarketType = marketType ?? string.Empty;
            OldestOddsAgeSeconds = oldestOddsAgeSeconds;
            PlayerName = playerName;
            PossiblyStale = possiblyStale;
            ProfitPercent = profitPercent;
            Sport = sport ?? string.Empty;
            StartTime = startTime;
            StatCategory = statCategory;
            Warnings = warnings;
            CreatedAtUtc = DateTime.Now;
            Legs = legs.ToList();
            foreach (var leg in Legs)
            {
                leg.AssignArbitrage(Id);
            }
        }

        /// <summary>Identificador SharpAPI de la oportunidad.</summary>
        public string Id { get; protected set; } = string.Empty;

        /// <summary>Momento en que SharpAPI detectó el arb.</summary>
        public DateTime? DetectedAt { get; protected set; }

        /// <summary>Beneficio neto estimado en porcentaje, tras comisiones.</summary>
        public decimal? EstimatedNetProfitPercent { get; protected set; }

        /// <summary>Indica si SharpAPI publicó un EV para esta oportunidad.</summary>
        public bool EvAvailable { get; protected set; }

        /// <summary>Porcentaje de valor esperado cuando está disponible.</summary>
        public decimal? EvPercentage { get; protected set; }

        /// <summary>Identificador del evento.</summary>
        public string EventId { get; protected set; } = string.Empty;

        /// <summary>Nombre visible del evento.</summary>
        public string EventName { get; protected set; } = string.Empty;

        /// <summary>Suma de probabilidades implícitas de las patas (porcentaje).</summary>
        public decimal? ImpliedTotal { get; protected set; }

        /// <summary>Indica si alguna pata usa línea alterna.</summary>
        public bool IsAlternateLine { get; protected set; }

        /// <summary>Indica si el evento está en vivo.</summary>
        public bool IsLive { get; protected set; }

        /// <summary>Indica si el mercado es un prop de jugador.</summary>
        public bool IsPlayerProp { get; protected set; }

        /// <summary>Slug de la liga.</summary>
        public string League { get; protected set; } = string.Empty;

        /// <summary>Nombre visible de la liga.</summary>
        public string? LeagueLabel { get; protected set; }

        /// <summary>Línea del mercado padre; nula si no aplica.</summary>
        public decimal? Line { get; protected set; }

        /// <summary>Indica si la línea no se ha movido desde la detección.</summary>
        public bool LineUnchanged { get; protected set; }

        /// <summary>Nombre visible del mercado.</summary>
        public string? MarketLabel { get; protected set; }

        /// <summary>Tipo de mercado (moneyline, team_total, etc.).</summary>
        public string MarketType { get; protected set; } = string.Empty;

        /// <summary>Edad en segundos de la cuota más vieja del arb.</summary>
        public decimal? OldestOddsAgeSeconds { get; protected set; }

        /// <summary>Nombre del jugador en props.</summary>
        public string? PlayerName { get; protected set; }

        /// <summary>Indica si SharpAPI considera las cuotas posiblemente viejas.</summary>
        public bool PossiblyStale { get; protected set; }

        /// <summary>Porcentaje de beneficio bruto del arb.</summary>
        public decimal? ProfitPercent { get; protected set; }

        /// <summary>Slug del deporte.</summary>
        public string Sport { get; protected set; } = string.Empty;

        /// <summary>Inicio del evento.</summary>
        public DateTime? StartTime { get; protected set; }

        /// <summary>Categoría estadística en props.</summary>
        public string? StatCategory { get; protected set; }

        /// <summary>Avisos de SharpAPI separados por coma.</summary>
        public string? Warnings { get; protected set; }

        /// <summary>Fecha y hora UTC exactas (hasta segundos) de inserción local.</summary>
        public DateTime CreatedAtUtc { get; protected set; }

        /// <summary>Patas del arb (dos o más casas/selecciones).</summary>
        public ICollection<ArbitrageOpportunityLeg> Legs { get; protected set; } = new List<ArbitrageOpportunityLeg>();
    }
}
