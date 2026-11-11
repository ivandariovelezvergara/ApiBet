using Domain.Entities;

namespace Aplication.DTOs
{
    /// <summary>
    /// Catálogo de bitácora de apuestas ejecutadas expuesto por GET /api/bets.
    /// </summary>
    public sealed class PlacedBetsCatalogDto
    {
        /// <summary>Lista de apuestas registradas.</summary>
        public required IReadOnlyList<PlacedBet> Data { get; init; }

        /// <summary>Resumen ejecutivo de ROI y CLV.</summary>
        public required PlacedBetsSummary Summary { get; init; }

        /// <summary>
        /// Proyecta el catálogo de dominio al DTO de respuesta.
        /// </summary>
        public static PlacedBetsCatalogDto FromDomain(PlacedBetsCatalog catalog)
        {
            return new PlacedBetsCatalogDto
            {
                Data = catalog.Bets,
                Summary = catalog.Summary
            };
        }
    }
}
