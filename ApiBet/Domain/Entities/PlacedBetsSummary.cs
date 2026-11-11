namespace Domain.Entities
{
    /// <summary>
    /// Resumen global de rendimiento real (ROI y CLV) de dbo.PlacedBets.
    /// </summary>
    public sealed class PlacedBetsSummary
    {
        /// <summary>Total de apuestas registradas.</summary>
        public int TotalBets { get; init; }

        /// <summary>Apuestas en curso (PENDING).</summary>
        public int PendingBets { get; init; }

        /// <summary>Apuestas liquidadas (WON, LOST, PUSH).</summary>
        public int ResolvedBets { get; init; }

        /// <summary>Apuestas ganadas (WON).</summary>
        public int WonBets { get; init; }

        /// <summary>Apuestas perdidas (LOST).</summary>
        public int LostBets { get; init; }

        /// <summary>Apuestas nulas o empatadas (PUSH / VOID).</summary>
        public int PushBets { get; init; }

        /// <summary>Volumen total apostado en COP.</summary>
        public decimal TotalStakedCop { get; init; }

        /// <summary>Volumen liquidado en COP.</summary>
        public decimal ResolvedStakedCop { get; init; }

        /// <summary>Exposición activa en apuestas pendientes en COP.</summary>
        public decimal PendingExposureCop { get; init; }

        /// <summary>Ganancia o pérdida neta real acumulada en COP.</summary>
        public decimal RealizedProfitCop { get; init; }

        /// <summary>Ganancia matemática esperada acumulada en COP.</summary>
        public decimal ExpectedProfitCop { get; init; }

        /// <summary>ROI real sobre volumen liquidado en porcentaje.</summary>
        public decimal RoiPct { get; init; }

        /// <summary>EV medio estimado al tomar las apuestas en porcentaje.</summary>
        public decimal AvgEvPct { get; init; }

        /// <summary>CLV medio en porcentaje.</summary>
        public decimal AvgClvPct { get; init; }

        /// <summary>Porcentaje de apuestas que batieron la línea de cierre (CLV &gt; 0).</summary>
        public decimal PositiveClvRatePct { get; init; }
    }
}
