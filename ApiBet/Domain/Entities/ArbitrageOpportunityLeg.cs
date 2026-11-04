namespace Domain.Entities
{
    /// <summary>
    /// Pata de una oportunidad de arbitraje de SharpAPI.
    /// </summary>
    public class ArbitrageOpportunityLeg
    {
        /// <summary>
        /// Constructor requerido por el mapeo de persistencia.
        /// </summary>
        protected ArbitrageOpportunityLeg()
        {
        }

        /// <summary>
        /// Crea una pata de arbitraje a partir del catálogo remoto.
        /// </summary>
        public ArbitrageOpportunityLeg(
            string? deepLink,
            string? externalEventId,
            string? externalMarketId,
            decimal? line,
            string? marketId,
            decimal? oddsAmerican,
            decimal? oddsDecimal,
            string? oddsId,
            decimal? oddsProbability,
            string selection,
            string? selectionId,
            string sportsbook,
            decimal? stakePercent)
        {
            Id = Guid.NewGuid().ToString("N");
            DeepLink = deepLink;
            ExternalEventId = externalEventId;
            ExternalMarketId = externalMarketId;
            Line = line;
            MarketId = marketId;
            OddsAmerican = oddsAmerican;
            OddsDecimal = oddsDecimal;
            OddsId = oddsId;
            OddsProbability = oddsProbability;
            Selection = selection ?? string.Empty;
            SelectionId = selectionId;
            Sportsbook = sportsbook ?? string.Empty;
            StakePercent = stakePercent;
        }

        /// <summary>
        /// Identificador único de la pata.
        /// </summary>
        public string Id { get; protected set; } = string.Empty;

        /// <summary>
        /// Identificador de la oportunidad padre.
        /// </summary>
        public string ArbitrageId { get; protected set; } = string.Empty;

        /// <summary>
        /// Enlace profundo de SharpAPI hacia la casa.
        /// </summary>
        public string? DeepLink { get; protected set; }

        /// <summary>
        /// Identificador externo del evento en la casa.
        /// </summary>
        public string? ExternalEventId { get; protected set; }

        /// <summary>
        /// Identificador externo del mercado en la casa.
        /// </summary>
        public string? ExternalMarketId { get; protected set; }

        /// <summary>
        /// Línea de esta pata; puede diferir de la línea padre.
        /// </summary>
        public decimal? Line { get; protected set; }

        /// <summary>
        /// Identificador de mercado en la casa.
        /// </summary>
        public string? MarketId { get; protected set; }

        /// <summary>
        /// Cuota americana.
        /// </summary>
        public decimal? OddsAmerican { get; protected set; }

        /// <summary>
        /// Cuota decimal.
        /// </summary>
        public decimal? OddsDecimal { get; protected set; }

        /// <summary>
        /// Identificador de la cuota en SharpAPI.
        /// </summary>
        public string? OddsId { get; protected set; }

        /// <summary>
        /// Probabilidad implícita de la cuota.
        /// </summary>
        public decimal? OddsProbability { get; protected set; }

        /// <summary>
        /// Selección a apostar.
        /// </summary>
        public string Selection { get; protected set; } = string.Empty;

        /// <summary>
        /// Identificador de la selección en la casa.
        /// </summary>
        public string? SelectionId { get; protected set; }

        /// <summary>
        /// Casa de apuestas de esta pata.
        /// </summary>
        public string Sportsbook { get; protected set; } = string.Empty;

        /// <summary>
        /// Porcentaje de la banca que SharpAPI asigna a esta pata.
        /// </summary>
        public decimal? StakePercent { get; protected set; }

        /// <summary>
        /// Asocia esta pata con la oportunidad padre.
        /// </summary>
        /// <param name="arbitrageId">Identificador de la fila padre.</param>
        public void AssignArbitrage(string arbitrageId)
        {
            ArbitrageId = arbitrageId;
        }
    }
}
