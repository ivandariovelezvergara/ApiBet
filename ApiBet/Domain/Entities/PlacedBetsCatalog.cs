namespace Domain.Entities
{
    /// <summary>
    /// Resultado de dbo.spr_PlacedBets_Get: bitácora de apuestas y resumen de CLV / ROI.
    /// </summary>
    public sealed class PlacedBetsCatalog
    {
        /// <summary>
        /// Inicializa el catálogo con la lista de apuestas y el resumen global.
        /// </summary>
        public PlacedBetsCatalog(
            IReadOnlyList<PlacedBet> bets,
            PlacedBetsSummary summary)
        {
            ArgumentNullException.ThrowIfNull(bets);
            ArgumentNullException.ThrowIfNull(summary);

            Bets = bets;
            Summary = summary;
        }

        /// <summary>Apuestas registradas.</summary>
        public IReadOnlyList<PlacedBet> Bets { get; }

        /// <summary>Resumen ejecutivo de ROI y CLV.</summary>
        public PlacedBetsSummary Summary { get; }
    }
}
