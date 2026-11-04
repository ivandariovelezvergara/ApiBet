namespace Domain.Entities
{
    /// <summary>
    /// Parámetros de dbo.spr_BestOdds_ValueBets, tomados de ValueModel.
    /// </summary>
    public sealed class ValueBetQuery
    {
        /// <summary>Banca actual en COP.</summary>
        public decimal Bankroll { get; init; }

        /// <summary>Máximo por apuesta individual en porcentaje.</summary>
        public decimal MaxStakePercent { get; init; }

        /// <summary>Tope de banca comprometida en la ventana.</summary>
        public decimal MaxTotalExposurePercent { get; init; }

        /// <summary>Fracción de Kelly.</summary>
        public decimal KellyFraction { get; init; }

        /// <summary>Cantidad máxima de picks.</summary>
        public int MaxPicks { get; init; }

        /// <summary>EV mínimo (fracción, 0.022 = 2.2 %).</summary>
        public decimal MinExpectedValue { get; init; }

        /// <summary>EV máximo (fracción, 0.16 = 16 %).</summary>
        public decimal MaxExpectedValue { get; init; }

        /// <summary>Cuota decimal mínima.</summary>
        public decimal MinOdds { get; init; }

        /// <summary>Cuota decimal máxima.</summary>
        public decimal MaxOdds { get; init; }

        /// <summary>Minutos mínimos hasta el inicio del evento.</summary>
        public int MinMinutesToStart { get; init; }

        /// <summary>Minutos máximos hasta el inicio del evento.</summary>
        public int MaxMinutesToStart { get; init; }

        /// <summary>Redondeo del stake en COP.</summary>
        public decimal StakeRoundToCop { get; init; }

        /// <summary>Stake mínimo viable en COP.</summary>
        public decimal MinStakeCop { get; init; }

        /// <summary>Si es verdadero, solo línea principal.</summary>
        public bool IsMainLine { get; init; }
    }
}
