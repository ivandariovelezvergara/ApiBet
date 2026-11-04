namespace Infraestructure.Persistence
{
    /// <summary>
    /// Identificadores de las filas de metadatos de sincronización.
    /// </summary>
    internal static class CatalogSyncKeys
    {
        /// <summary>
        /// Catálogo de deportes.
        /// </summary>
        public const int Sports = 1;

        /// <summary>
        /// Catálogo de ligas.
        /// </summary>
        public const int Leagues = 2;

        /// <summary>
        /// Catálogo de casas de apuestas.
        /// </summary>
        public const int Sportsbooks = 3;

        /// <summary>
        /// Catálogo de mercados.
        /// </summary>
        public const int Markets = 4;

        /// <summary>
        /// Snapshot de mejores cuotas.
        /// </summary>
        public const int BestOdds = 5;

        /// <summary>
        /// Snapshot de cuotas de mercado (GET /odds).
        /// </summary>
        public const int MarketOdds = 6;

        /// <summary>
        /// Snapshot de oportunidades de arbitraje (GET /opportunities/arbitrage).
        /// </summary>
        public const int ArbitrageOpportunities = 7;

        /// <summary>
        /// Sincronización incremental de cuotas en MarketOddsDelta (GET /odds/delta).
        /// </summary>
        public const int MarketOddsDelta = 8;
    }
}
