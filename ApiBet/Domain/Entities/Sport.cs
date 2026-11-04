namespace Domain.Entities
{
    /// <summary>
    /// Deporte del catálogo de SharpAPI, incluyendo sus ligas asociadas.
    /// </summary>
    public class Sport
    {
        /// <summary>
        /// Constructor requerido por el mapeo de persistencia.
        /// </summary>
        protected Sport()
        {
        }

        /// <summary>
        /// Crea un deporte de dominio a partir de los datos del catálogo.
        /// </summary>
        /// <param name="id">Identificador del deporte (por ejemplo, tennis).</param>
        /// <param name="name">Nombre visible del deporte.</param>
        /// <param name="numericalId">Identificador numérico estable; puede no existir.</param>
        /// <param name="eventCount">Cantidad total de eventos con cuotas.</param>
        /// <param name="liveCount">Cantidad de eventos en vivo.</param>
        /// <param name="leagueCodes">Códigos de ligas asociadas al deporte.</param>
        public Sport(
            string id,
            string name,
            int? numericalId,
            int eventCount,
            int liveCount,
            IEnumerable<string> leagueCodes)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(leagueCodes);

            Id = id;
            Name = name;
            NumericalId = numericalId;
            EventCount = eventCount;
            LiveCount = liveCount;
            Leagues = leagueCodes
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(code => new SportLeague(id, code))
                .ToList();
        }

        /// <summary>
        /// Identificador del deporte, utilizado como filtro en otras consultas de SharpAPI.
        /// </summary>
        public string Id { get; protected set; } = string.Empty;

        /// <summary>
        /// Nombre legible del deporte.
        /// </summary>
        public string Name { get; protected set; } = string.Empty;

        /// <summary>
        /// Clave numérica estable del deporte. Es opcional cuando SharpAPI aún no lo ha mapeado.
        /// </summary>
        public int? NumericalId { get; protected set; }

        /// <summary>
        /// Total de eventos disponibles con cuotas.
        /// </summary>
        public int EventCount { get; protected set; }

        /// <summary>
        /// Eventos que se encuentran actualmente en vivo.
        /// </summary>
        public int LiveCount { get; protected set; }

        /// <summary>
        /// Ligas que pertenecen a este deporte.
        /// </summary>
        public ICollection<SportLeague> Leagues { get; protected set; } = new List<SportLeague>();
    }
}
