namespace Domain.Entities
{
    /// <summary>
    /// Pick de value bet devuelto por dbo.spr_BestOdds_ValueBets.
    /// </summary>
    public sealed class ValueBetPick
    {
        /// <summary>Orden del pick en la cartera.</summary>
        public int Pick { get; init; }

        /// <summary>Señal cualitativa del valor.</summary>
        public required string Senal { get; init; }

        /// <summary>Deporte.</summary>
        public required string Deporte { get; init; }

        /// <summary>Liga.</summary>
        public required string Liga { get; init; }

        /// <summary>Partido.</summary>
        public required string Partido { get; init; }

        /// <summary>Minutos hasta el inicio.</summary>
        public int MinutosFaltantes { get; init; }

        /// <summary>Mercado.</summary>
        public required string Mercado { get; init; }

        /// <summary>Selección a apostar.</summary>
        public required string ApostarA { get; init; }

        /// <summary>Línea, si aplica.</summary>
        public decimal? Linea { get; init; }

        /// <summary>Casa de apuestas.</summary>
        public required string Casa { get; init; }

        /// <summary>Cuota decimal actual.</summary>
        public decimal CuotaActual { get; init; }

        /// <summary>Cuota mínima rentable según el EV mínimo.</summary>
        public decimal CuotaMinimaRentable { get; init; }

        /// <summary>Probabilidad de consenso en porcentaje.</summary>
        public decimal ProbConsensoPct { get; init; }

        /// <summary>Probabilidad implícita de la cuota en porcentaje.</summary>
        public decimal ProbImplicitaPct { get; init; }

        /// <summary>Expected value en porcentaje.</summary>
        public decimal EvPct { get; init; }

        /// <summary>Kelly aplicado en porcentaje.</summary>
        public decimal KellyAplicadoPct { get; init; }

        /// <summary>Stake en COP.</summary>
        public decimal StakeCop { get; init; }

        /// <summary>Ganancia esperada en COP.</summary>
        public decimal GananciaEsperadaCop { get; init; }

        /// <summary>Exposición acumulada en COP.</summary>
        public decimal RiesgoAcumuladoCop { get; init; }

        /// <summary>Instrucción de apuesta.</summary>
        public required string Instruccion { get; init; }
    }
}
