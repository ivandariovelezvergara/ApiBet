namespace Domain.Entities
{
    /// <summary>
    /// Resultado de dbo.spr_BestOdds_ValueBets: picks y resumen.
    /// </summary>
    public sealed class ValueBetCatalog
    {
        /// <summary>
        /// Crea el catálogo con ambos result sets del SP.
        /// </summary>
        public ValueBetCatalog(
            IReadOnlyList<ValueBetPick> picks,
            IReadOnlyList<ValueBetSummary> summaries)
        {
            ArgumentNullException.ThrowIfNull(picks);
            ArgumentNullException.ThrowIfNull(summaries);
            Picks = picks;
            Summaries = summaries;
        }

        /// <summary>Picks seleccionados de la cartera.</summary>
        public IReadOnlyList<ValueBetPick> Picks { get; }

        /// <summary>Resumen por deporte y liga.</summary>
        public IReadOnlyList<ValueBetSummary> Summaries { get; }
    }
}
