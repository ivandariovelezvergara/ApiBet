namespace Domain.Entities
{
    /// <summary>
    /// Cuota retirada informada por GET /odds/delta de SharpAPI.
    /// </summary>
    public sealed class RemovedOdd
    {
        /// <summary>
        /// Crea el identificador retirado con los metadatos opcionales del feed delta.
        /// </summary>
        public RemovedOdd(
            string id,
            string? sportsbook,
            DateTime? removedAt,
            DateTime? eventStart,
            bool wasLive,
            bool boundary)
        {
            Id = id;
            Sportsbook = sportsbook;
            RemovedAt = removedAt;
            EventStart = eventStart;
            WasLive = wasLive;
            Boundary = boundary;
        }

        /// <summary>
        /// Identificador de la cuota retirada.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Casa de apuestas informada por SharpAPI.
        /// </summary>
        public string? Sportsbook { get; }

        /// <summary>
        /// Marca de tiempo UTC en la que se retiró la cuota.
        /// </summary>
        public DateTime? RemovedAt { get; }

        /// <summary>
        /// Inicio del evento asociado.
        /// </summary>
        public DateTime? EventStart { get; }

        /// <summary>
        /// Indica si la cuota estaba en vivo al retirarse.
        /// </summary>
        public bool WasLive { get; }

        /// <summary>
        /// Indica si el retiro corresponde a un recorte de cobertura histórica.
        /// </summary>
        public bool Boundary { get; }
    }
}
