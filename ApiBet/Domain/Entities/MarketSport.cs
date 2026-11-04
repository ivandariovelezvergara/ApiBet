namespace Domain.Entities
{
    /// <summary>
    /// Deporte asociado a un tipo de mercado.
    /// </summary>
    public class MarketSport
    {
        /// <summary>
        /// Constructor requerido por el mapeo de persistencia.
        /// </summary>
        protected MarketSport()
        {
        }

        /// <summary>
        /// Crea la relación entre un mercado y un deporte.
        /// </summary>
        /// <param name="marketId">Identificador del mercado padre.</param>
        /// <param name="sportId">Identificador del deporte (por ejemplo, tennis).</param>
        public MarketSport(string marketId, string sportId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(marketId);
            ArgumentException.ThrowIfNullOrWhiteSpace(sportId);

            MarketId = marketId;
            SportId = sportId;
        }

        /// <summary>
        /// Identificador del mercado.
        /// </summary>
        public string MarketId { get; protected set; } = string.Empty;

        /// <summary>
        /// Identificador del deporte informado por SharpAPI.
        /// </summary>
        public string SportId { get; protected set; } = string.Empty;
    }
}
