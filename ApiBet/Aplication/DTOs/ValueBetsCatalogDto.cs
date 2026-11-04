using Domain.Entities;

namespace Aplication.DTOs
{
    /// <summary>
    /// Catálogo de value bets expuesto por GET /api/odds/best.
    /// </summary>
    public sealed class ValueBetsCatalogDto
    {
        /// <summary>Picks de la cartera.</summary>
        public required IReadOnlyList<ValueBetPick> Picks { get; init; }

        /// <summary>Resumen ejecutivo por deporte y liga.</summary>
        public required IReadOnlyList<ValueBetSummary> Summaries { get; init; }

        /// <summary>
        /// Proyecta el resultado del SP al DTO de aplicación.
        /// </summary>
        public static ValueBetsCatalogDto FromDomain(ValueBetCatalog catalog)
        {
            return new ValueBetsCatalogDto
            {
                Picks = catalog.Picks,
                Summaries = catalog.Summaries
            };
        }
    }
}
