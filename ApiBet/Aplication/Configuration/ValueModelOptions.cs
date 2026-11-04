using Domain.Entities;

namespace Aplication.Configuration
{
    /// <summary>
    /// Parámetros del modelo +EV y Third-Kelly para dbo.spr_BestOdds_ValueBets.
    /// </summary>
    public sealed class ValueModelOptions
    {
        /// <summary>
        /// Nombre de la sección en appsettings.
        /// </summary>
        public const string SectionName = "ValueModel";

        /// <summary>
        /// Banca total en COP.
        /// </summary>
        public decimal BankrollCop { get; set; } = 1_000_000m;

        /// <summary>
        /// Tope de riesgo por jugada como porcentaje de la banca (1.75 = 1.75 %).
        /// </summary>
        public decimal MaxStakePercent { get; set; } = 1.75m;

        /// <summary>
        /// Tope de banca comprometida en la ventana de 4 h (18 = 18 %).
        /// </summary>
        public decimal MaxTotalExposurePercent { get; set; } = 18m;

        /// <summary>
        /// Fracción de Kelly (0.30 = Third-Kelly).
        /// </summary>
        public decimal KellyFraction { get; set; } = 0.30m;

        /// <summary>
        /// Máximo de picks que se devuelven.
        /// </summary>
        public int MaxPicks { get; set; } = 10;

        /// <summary>
        /// EV mínimo del SP (0.022 = 2.2 %).
        /// </summary>
        public decimal MinExpectedValue { get; set; } = 0.022m;

        /// <summary>
        /// EV máximo del SP (0.16 = 16 %).
        /// </summary>
        public decimal MaxExpectedValue { get; set; } = 0.16m;

        /// <summary>
        /// Cuota decimal mínima.
        /// </summary>
        public decimal MinOdds { get; set; } = 2.10m;

        /// <summary>
        /// Cuota decimal máxima.
        /// </summary>
        public decimal MaxOdds { get; set; } = 3.50m;

        /// <summary>
        /// Minutos mínimos antes del evento.
        /// </summary>
        public int MinMinutesToStart { get; set; } = 45;

        /// <summary>
        /// Minutos máximos antes del evento (4 h = 240).
        /// </summary>
        public int MaxMinutesToStart { get; set; } = 240;

        /// <summary>
        /// Redondeo anti-limitación en COP.
        /// </summary>
        public decimal StakeRoundToCop { get; set; } = 1000m;

        /// <summary>
        /// Apuesta mínima viable en COP.
        /// </summary>
        public decimal MinStakeCop { get; set; } = 5000m;

        /// <summary>
        /// Si es verdadero, solo línea principal.
        /// </summary>
        public bool IsMainLine { get; set; } = true;

        /// <summary>
        /// Arma el query de dbo.spr_BestOdds_ValueBets.
        /// </summary>
        public ValueBetQuery ToValueBetQuery()
        {
            return new ValueBetQuery
            {
                Bankroll = BankrollCop,
                MaxStakePercent = MaxStakePercent,
                MaxTotalExposurePercent = MaxTotalExposurePercent,
                KellyFraction = KellyFraction,
                MaxPicks = MaxPicks,
                MinExpectedValue = MinExpectedValue,
                MaxExpectedValue = MaxExpectedValue,
                MinOdds = MinOdds,
                MaxOdds = MaxOdds,
                MinMinutesToStart = MinMinutesToStart,
                MaxMinutesToStart = MaxMinutesToStart,
                StakeRoundToCop = StakeRoundToCop,
                MinStakeCop = MinStakeCop,
                IsMainLine = IsMainLine
            };
        }
    }
}
