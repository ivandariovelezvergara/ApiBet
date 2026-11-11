namespace Domain.Entities
{
    /// <summary>
    /// Datos de entrada para registrar una apuesta ejecutada en dbo.PlacedBets.
    /// </summary>
    public sealed class NewPlacedBet
    {
        /// <summary>Tipo de oportunidad (VALUE_BET o ARBITRAGE).</summary>
        public string BetKind { get; init; } = "VALUE_BET";

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

        /// <summary>Casa de apuestas donde se colocó.</summary>
        public required string Casa { get; init; }

        /// <summary>Cuota decimal tomada.</summary>
        public decimal CuotaTomada { get; init; }

        /// <summary>Cuota mínima rentable sugerida.</summary>
        public decimal? CuotaMinimaRentable { get; init; }

        /// <summary>Probabilidad de consenso al momento del pick.</summary>
        public decimal? ProbConsensoTomadaPct { get; init; }

        /// <summary>EV estimado en porcentaje.</summary>
        public decimal EvEstimadoPct { get; init; }

        /// <summary>Monto apostado en COP.</summary>
        public decimal StakeCop { get; init; }

        /// <summary>Ganancia esperada en COP.</summary>
        public decimal? GananciaEsperadaCop { get; init; }
    }
}
