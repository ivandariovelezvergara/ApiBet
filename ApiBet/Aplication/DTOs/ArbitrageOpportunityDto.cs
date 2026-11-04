using Domain.Entities;

namespace Aplication.DTOs
{
    /// <summary>
    /// Oportunidad de arbitraje almacenada localmente.
    /// </summary>
    public sealed class ArbitrageOpportunityDto
    {
        /// <summary>Identificador SharpAPI de la oportunidad.</summary>
        public required string Id { get; init; }

        /// <summary>Momento en que SharpAPI detectó el arb.</summary>
        public DateTime? DetectedAt { get; init; }

        /// <summary>Beneficio neto estimado en porcentaje.</summary>
        public decimal? EstimatedNetProfitPercent { get; init; }

        /// <summary>Indica si hay EV publicado.</summary>
        public bool EvAvailable { get; init; }

        /// <summary>Porcentaje de valor esperado cuando está disponible.</summary>
        public decimal? EvPercentage { get; init; }

        /// <summary>Identificador del evento.</summary>
        public required string EventId { get; init; }

        /// <summary>Nombre visible del evento.</summary>
        public required string EventName { get; init; }

        /// <summary>Suma de probabilidades implícitas (porcentaje).</summary>
        public decimal? ImpliedTotal { get; init; }

        /// <summary>Indica si alguna pata usa línea alterna.</summary>
        public bool IsAlternateLine { get; init; }

        /// <summary>Indica si el evento está en vivo.</summary>
        public bool IsLive { get; init; }

        /// <summary>Indica si el mercado es un prop de jugador.</summary>
        public bool IsPlayerProp { get; init; }

        /// <summary>Slug de la liga.</summary>
        public required string League { get; init; }

        /// <summary>Nombre visible de la liga.</summary>
        public string? LeagueLabel { get; init; }

        /// <summary>Línea del mercado padre.</summary>
        public decimal? Line { get; init; }

        /// <summary>Indica si la línea no se ha movido.</summary>
        public bool LineUnchanged { get; init; }

        /// <summary>Nombre visible del mercado.</summary>
        public string? MarketLabel { get; init; }

        /// <summary>Tipo de mercado.</summary>
        public required string MarketType { get; init; }

        /// <summary>Edad en segundos de la cuota más vieja.</summary>
        public decimal? OldestOddsAgeSeconds { get; init; }

        /// <summary>Nombre del jugador en props.</summary>
        public string? PlayerName { get; init; }

        /// <summary>Indica si las cuotas pueden estar viejas.</summary>
        public bool PossiblyStale { get; init; }

        /// <summary>Porcentaje de beneficio bruto del arb.</summary>
        public decimal? ProfitPercent { get; init; }

        /// <summary>Slug del deporte.</summary>
        public required string Sport { get; init; }

        /// <summary>Inicio del evento.</summary>
        public DateTime? StartTime { get; init; }

        /// <summary>Categoría estadística en props.</summary>
        public string? StatCategory { get; init; }

        /// <summary>Avisos de SharpAPI.</summary>
        public IReadOnlyList<string> Warnings { get; init; } = [];

        /// <summary>Patas del arb.</summary>
        public required IReadOnlyList<ArbitrageLegDto> Legs { get; init; }

        /// <summary>
        /// Proyecta la entidad de dominio al DTO de aplicación.
        /// </summary>
        public static ArbitrageOpportunityDto FromDomain(ArbitrageOpportunity opportunity)
        {
            return new ArbitrageOpportunityDto
            {
                Id = opportunity.Id,
                DetectedAt = opportunity.DetectedAt,
                EstimatedNetProfitPercent = opportunity.EstimatedNetProfitPercent,
                EvAvailable = opportunity.EvAvailable,
                EvPercentage = opportunity.EvPercentage,
                EventId = opportunity.EventId,
                EventName = opportunity.EventName,
                ImpliedTotal = opportunity.ImpliedTotal,
                IsAlternateLine = opportunity.IsAlternateLine,
                IsLive = opportunity.IsLive,
                IsPlayerProp = opportunity.IsPlayerProp,
                League = opportunity.League,
                LeagueLabel = opportunity.LeagueLabel,
                Line = opportunity.Line,
                LineUnchanged = opportunity.LineUnchanged,
                MarketLabel = opportunity.MarketLabel,
                MarketType = opportunity.MarketType,
                OldestOddsAgeSeconds = opportunity.OldestOddsAgeSeconds,
                PlayerName = opportunity.PlayerName,
                PossiblyStale = opportunity.PossiblyStale,
                ProfitPercent = opportunity.ProfitPercent,
                Sport = opportunity.Sport,
                StartTime = opportunity.StartTime,
                StatCategory = opportunity.StatCategory,
                Warnings = SplitWarnings(opportunity.Warnings),
                Legs = opportunity.Legs
                    .Select(ArbitrageLegDto.FromDomain)
                    .ToArray()
            };
        }

        /// <summary>
        /// Separa los avisos persistidos como texto CSV.
        /// </summary>
        private static IReadOnlyList<string> SplitWarnings(string? warnings)
        {
            if (string.IsNullOrWhiteSpace(warnings))
            {
                return [];
            }

            return warnings
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToArray();
        }
    }
}
