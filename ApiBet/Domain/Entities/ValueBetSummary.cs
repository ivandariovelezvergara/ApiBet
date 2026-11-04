namespace Domain.Entities
{
    /// <summary>
    /// Resumen ejecutivo por deporte y liga de dbo.spr_BestOdds_ValueBets.
    /// </summary>
    public sealed class ValueBetSummary
    {
        /// <summary>Deporte.</summary>
        public required string Deporte { get; init; }

        /// <summary>Liga.</summary>
        public required string Liga { get; init; }

        /// <summary>Partidos con valor.</summary>
        public int PartidosConValor { get; init; }

        /// <summary>Picks totales del grupo.</summary>
        public int PicksTotales { get; init; }

        /// <summary>Surebets detectadas.</summary>
        public int SurebetsDetectadas { get; init; }

        /// <summary>EV medio en porcentaje.</summary>
        public decimal EvMedioPct { get; init; }

        /// <summary>Cuota media.</summary>
        public decimal CuotaMedia { get; init; }
    }
}
