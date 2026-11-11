namespace Domain.Entities
{
    /// <summary>
    /// Apuesta ejecutada y registrada en dbo.PlacedBets con seguimiento de CLV y resultado.
    /// </summary>
    public sealed class PlacedBet
    {
        /// <summary>Identificador único de la apuesta registrada.</summary>
        public int Id { get; init; }

        /// <summary>Tipo de oportunidad (VALUE_BET o ARBITRAGE).</summary>
        public required string BetKind { get; init; }

        /// <summary>Deporte.</summary>
        public required string Deporte { get; init; }

        /// <summary>Liga.</summary>
        public required string Liga { get; init; }

        /// <summary>Partido.</summary>
        public required string Partido { get; init; }

        /// <summary>Mercado.</summary>
        public required string Mercado { get; init; }

        /// <summary>Selección apostada.</summary>
        public required string Seleccion { get; init; }

        /// <summary>Línea del mercado, si aplica.</summary>
        public decimal? Linea { get; init; }

        /// <summary>Casa de apuestas donde se ejecutó.</summary>
        public required string Casa { get; init; }

        /// <summary>Cuota decimal tomada al apostar.</summary>
        public decimal CuotaTomada { get; init; }

        /// <summary>Cuota mínima rentable estimada por el modelo.</summary>
        public decimal? CuotaMinimaRentable { get; init; }

        /// <summary>Probabilidad real de consenso al momento de tomar el pick.</summary>
        public decimal? ProbConsensoTomadaPct { get; init; }

        /// <summary>EV estimado en porcentaje al tomar la apuesta.</summary>
        public decimal EvEstimadoPct { get; init; }

        /// <summary>Monto apostado en COP.</summary>
        public decimal StakeCop { get; init; }

        /// <summary>Ganancia matemática esperada en COP.</summary>
        public decimal GananciaEsperadaCop { get; init; }

        /// <summary>Última cuota de consenso / mercado observada antes del inicio (línea de cierre).</summary>
        public decimal? UltimaCuotaMercado { get; init; }

        /// <summary>Última probabilidad de consenso observada antes del inicio.</summary>
        public decimal? UltimaProbConsensoPct { get; init; }

        /// <summary>Closing Line Value (CLV) en porcentaje: ((CuotaTomada / UltimaCuotaMercado) - 1) * 100.</summary>
        public decimal? ClvCuotaPct { get; init; }

        /// <summary>Estado actual: PENDING, WON, LOST, PUSH, VOID.</summary>
        public required string Estado { get; init; }

        /// <summary>Utilidad o pérdida neta real en COP una vez liquidada.</summary>
        public decimal? ProfitRealCop { get; init; }

        /// <summary>Fecha y hora Colombia en que se registró la apuesta.</summary>
        public DateTime PlacedAtCol { get; init; }

        /// <summary>Fecha y hora Colombia en que se liquidó la apuesta.</summary>
        public DateTime? ClosedAtCol { get; init; }
    }
}
