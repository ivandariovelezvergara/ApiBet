namespace Domain.Entities
{
    /// <summary>
    /// Boleta de arbitraje a 2 vías devuelta por dbo.spr_ArbitrageOpportunities.
    /// </summary>
    public sealed class ArbitrageBetRow
    {
        /// <summary>
        /// Crea la boleta con los montos e instrucción ya calculados.
        /// </summary>
        public ArbitrageBetRow(
            string deporte,
            string league,
            string partido,
            DateTime? eventStartTime,
            bool enVivo,
            DateTime createdAtUtc,
            string betType,
            string casaApuesta1,
            string equipo1,
            decimal cuota1,
            decimal monto1,
            string casaApuesta2,
            string equipo2,
            decimal cuota2,
            decimal monto2,
            decimal banca,
            string instruccion,
            decimal gananciaEsperada,
            decimal totalNetoGanado)
        {
            Deporte = deporte;
            League = league;
            Partido = partido;
            EventStartTime = eventStartTime;
            EnVivo = enVivo;
            CreatedAtUtc = createdAtUtc;
            BetType = betType;
            CasaApuesta1 = casaApuesta1;
            Equipo1 = equipo1;
            Cuota1 = cuota1;
            Monto1 = monto1;
            CasaApuesta2 = casaApuesta2;
            Equipo2 = equipo2;
            Cuota2 = cuota2;
            Monto2 = monto2;
            Banca = banca;
            Instruccion = instruccion;
            GananciaEsperada = gananciaEsperada;
            TotalNetoGanado = totalNetoGanado;
        }

        /// <summary>Slug del deporte.</summary>
        public string Deporte { get; }

        /// <summary>Slug de la liga.</summary>
        public string League { get; }

        /// <summary>Nombre del partido.</summary>
        public string Partido { get; }

        /// <summary>Inicio del evento.</summary>
        public DateTime? EventStartTime { get; }

        /// <summary>Indica si el partido está en vivo.</summary>
        public bool EnVivo { get; }

        /// <summary>Fecha y hora UTC exactas de inserción local.</summary>
        public DateTime CreatedAtUtc { get; }

        /// <summary>Tipo de mercado y línea si aplica.</summary>
        public string BetType { get; }

        /// <summary>Casa de la primera pata.</summary>
        public string CasaApuesta1 { get; }

        /// <summary>Selección de la primera pata.</summary>
        public string Equipo1 { get; }

        /// <summary>Cuota decimal de la primera pata.</summary>
        public decimal Cuota1 { get; }

        /// <summary>Monto en COP de la primera pata (múltiplo de 100).</summary>
        public decimal Monto1 { get; }

        /// <summary>Casa de la segunda pata.</summary>
        public string CasaApuesta2 { get; }

        /// <summary>Selección de la segunda pata.</summary>
        public string Equipo2 { get; }

        /// <summary>Cuota decimal de la segunda pata.</summary>
        public decimal Cuota2 { get; }

        /// <summary>Monto en COP de la segunda pata (múltiplo de 100).</summary>
        public decimal Monto2 { get; }

        /// <summary>Banca total repartida.</summary>
        public decimal Banca { get; }

        /// <summary>Frase con las dos apuestas.</summary>
        public string Instruccion { get; }

        /// <summary>Ganancia neta garantizada.</summary>
        public decimal GananciaEsperada { get; }

        /// <summary>Cobro total (banca + ganancia).</summary>
        public decimal TotalNetoGanado { get; }
    }
}
