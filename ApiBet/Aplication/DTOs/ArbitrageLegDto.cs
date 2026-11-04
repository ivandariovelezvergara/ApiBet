using Domain.Entities;

namespace Aplication.DTOs
{
    /// <summary>
    /// Pata de un arb almacenado localmente.
    /// </summary>
    public sealed class ArbitrageLegDto
    {
        /// <summary>Enlace profundo de SharpAPI hacia la casa.</summary>
        public string? DeepLink { get; init; }

        /// <summary>Identificador externo del evento en la casa.</summary>
        public string? ExternalEventId { get; init; }

        /// <summary>Identificador externo del mercado en la casa.</summary>
        public string? ExternalMarketId { get; init; }

        /// <summary>Línea de esta pata.</summary>
        public decimal? Line { get; init; }

        /// <summary>Identificador de mercado en la casa.</summary>
        public string? MarketId { get; init; }

        /// <summary>Cuota americana.</summary>
        public decimal? OddsAmerican { get; init; }

        /// <summary>Cuota decimal.</summary>
        public decimal? OddsDecimal { get; init; }

        /// <summary>Identificador de la cuota en SharpAPI.</summary>
        public string? OddsId { get; init; }

        /// <summary>Probabilidad implícita de la cuota.</summary>
        public decimal? OddsProbability { get; init; }

        /// <summary>Selección a apostar.</summary>
        public required string Selection { get; init; }

        /// <summary>Identificador de la selección en la casa.</summary>
        public string? SelectionId { get; init; }

        /// <summary>Casa de apuestas de esta pata.</summary>
        public required string Sportsbook { get; init; }

        /// <summary>Porcentaje de la banca asignado a esta pata.</summary>
        public decimal? StakePercent { get; init; }

        /// <summary>
        /// Proyecta la entidad de dominio al DTO de aplicación.
        /// </summary>
        public static ArbitrageLegDto FromDomain(ArbitrageOpportunityLeg leg)
        {
            return new ArbitrageLegDto
            {
                DeepLink = leg.DeepLink,
                ExternalEventId = leg.ExternalEventId,
                ExternalMarketId = leg.ExternalMarketId,
                Line = leg.Line,
                MarketId = leg.MarketId,
                OddsAmerican = leg.OddsAmerican,
                OddsDecimal = leg.OddsDecimal,
                OddsId = leg.OddsId,
                OddsProbability = leg.OddsProbability,
                Selection = leg.Selection,
                SelectionId = leg.SelectionId,
                Sportsbook = leg.Sportsbook,
                StakePercent = leg.StakePercent
            };
        }
    }
}
