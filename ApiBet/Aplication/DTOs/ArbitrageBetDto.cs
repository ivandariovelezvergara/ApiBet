using Domain.Entities;

namespace Aplication.DTOs
{
    /// <summary>
    /// Boleta de arbitraje alineada con dbo.spr_ArbitrageOpportunities.
    /// </summary>
    public sealed class ArbitrageBetDto
    {
        /// <summary>Slug del deporte.</summary>
        public required string Deporte { get; init; }

        /// <summary>Slug de la liga.</summary>
        public required string League { get; init; }

        /// <summary>Nombre del partido.</summary>
        public required string Partido { get; init; }

        /// <summary>Inicio del evento.</summary>
        public DateTime? EventStartTime { get; init; }

        /// <summary>Indica si el partido está en vivo.</summary>
        public bool EnVivo { get; init; }

        /// <summary>Fecha y hora UTC exactas de inserción local.</summary>
        public DateTime CreatedAtUtc { get; init; }

        /// <summary>Tipo de mercado y línea si aplica.</summary>
        public required string BetType { get; init; }

        /// <summary>Casa de la primera pata.</summary>
        public required string CasaApuesta1 { get; init; }

        /// <summary>Selección de la primera pata.</summary>
        public required string Equipo1 { get; init; }

        /// <summary>Cuota decimal de la primera pata.</summary>
        public decimal Cuota1 { get; init; }

        /// <summary>Monto en COP de la primera pata (múltiplo de 100).</summary>
        public decimal Monto1 { get; init; }

        /// <summary>Casa de la segunda pata.</summary>
        public required string CasaApuesta2 { get; init; }

        /// <summary>Selección de la segunda pata.</summary>
        public required string Equipo2 { get; init; }

        /// <summary>Cuota decimal de la segunda pata.</summary>
        public decimal Cuota2 { get; init; }

        /// <summary>Monto en COP de la segunda pata (múltiplo de 100).</summary>
        public decimal Monto2 { get; init; }

        /// <summary>Banca total repartida.</summary>
        public decimal Banca { get; init; }

        /// <summary>Frase con las dos apuestas.</summary>
        public required string Instruccion { get; init; }

        /// <summary>Ganancia neta garantizada.</summary>
        public decimal GananciaEsperada { get; init; }

        /// <summary>Cobro total (banca + ganancia).</summary>
        public decimal TotalNetoGanado { get; init; }

        /// <summary>
        /// Proyecta la fila del SP al DTO de aplicación.
        /// </summary>
        public static ArbitrageBetDto FromDomain(ArbitrageBetRow row)
        {
            return new ArbitrageBetDto
            {
                Deporte = row.Deporte,
                League = row.League,
                Partido = row.Partido,
                EventStartTime = row.EventStartTime,
                EnVivo = row.EnVivo,
                CreatedAtUtc = row.CreatedAtUtc,
                BetType = row.BetType,
                CasaApuesta1 = row.CasaApuesta1,
                Equipo1 = row.Equipo1,
                Cuota1 = row.Cuota1,
                Monto1 = row.Monto1,
                CasaApuesta2 = row.CasaApuesta2,
                Equipo2 = row.Equipo2,
                Cuota2 = row.Cuota2,
                Monto2 = row.Monto2,
                Banca = row.Banca,
                Instruccion = row.Instruccion,
                GananciaEsperada = row.GananciaEsperada,
                TotalNetoGanado = row.TotalNetoGanado
            };
        }
    }
}
