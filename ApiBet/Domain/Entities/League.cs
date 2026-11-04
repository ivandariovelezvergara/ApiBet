namespace Domain.Entities
{
    /// <summary>
    /// Liga del catálogo de SharpAPI, asociada a un deporte.
    /// </summary>
    public class League
    {
        /// <summary>
        /// Constructor requerido por el mapeo de persistencia.
        /// </summary>
        protected League()
        {
        }

        /// <summary>
        /// Crea una liga de dominio a partir de los datos del catálogo.
        /// </summary>
        /// <param name="id">Identificador de la liga (por ejemplo, atp).</param>
        /// <param name="displayName">Nombre visible de la liga.</param>
        /// <param name="numericalId">Identificador numérico estable; puede no existir.</param>
        /// <param name="sportId">Identificador del deporte al que pertenece.</param>
        /// <param name="eventCount">Cantidad total de eventos con cuotas.</param>
        /// <param name="liveCount">Cantidad de eventos en vivo.</param>
        public League(
            string id,
            string displayName,
            int? numericalId,
            string sportId,
            int eventCount,
            int liveCount)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
            ArgumentException.ThrowIfNullOrWhiteSpace(sportId);

            Id = id;
            DisplayName = displayName;
            NumericalId = numericalId;
            SportId = sportId;
            EventCount = eventCount;
            LiveCount = liveCount;
        }

        /// <summary>
        /// Identificador de la liga, utilizado como filtro en otras consultas de SharpAPI.
        /// </summary>
        public string Id { get; protected set; } = string.Empty;

        /// <summary>
        /// Nombre visible de la liga.
        /// </summary>
        public string DisplayName { get; protected set; } = string.Empty;

        /// <summary>
        /// Clave numérica estable de la liga. Es opcional cuando SharpAPI aún no la ha mapeado.
        /// </summary>
        public int? NumericalId { get; protected set; }

        /// <summary>
        /// Identificador del deporte al que pertenece la liga.
        /// </summary>
        public string SportId { get; protected set; } = string.Empty;

        /// <summary>
        /// Total de eventos disponibles con cuotas.
        /// </summary>
        public int EventCount { get; protected set; }

        /// <summary>
        /// Eventos que se encuentran actualmente en vivo.
        /// </summary>
        public int LiveCount { get; protected set; }
    }
}
