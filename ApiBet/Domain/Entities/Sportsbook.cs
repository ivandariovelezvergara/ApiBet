namespace Domain.Entities
{
    /// <summary>
    /// Casa de apuestas del catálogo de SharpAPI.
    /// </summary>
    public class Sportsbook
    {
        /// <summary>
        /// Constructor requerido por el mapeo de persistencia.
        /// </summary>
        protected Sportsbook()
        {
        }

        /// <summary>
        /// Crea una casa de apuestas de dominio a partir de los datos del catálogo.
        /// </summary>
        /// <param name="id">Identificador único para filtros de API.</param>
        /// <param name="numericalId">Identificador numérico estable; puede no existir.</param>
        /// <param name="displayName">Nombre legible de la casa.</param>
        /// <param name="shortName">Nombre corto para mostrar.</param>
        /// <param name="category">Categoría del libro (por ejemplo, major_us).</param>
        /// <param name="hasLiveOdds">Indica si ofrece cuotas en vivo.</param>
        /// <param name="hasPlayerProps">Indica si ofrece mercados de jugador.</param>
        /// <param name="hasLimits">Indica si publica límites; nulo si aún no se observó.</param>
        /// <param name="regions">Regiones disponibles (US, UK, EU, AU, GLOBAL).</param>
        /// <param name="isSharp">Indica si se considera un mercado sharp.</param>
        /// <param name="status">Estado operativo (active o inactive).</param>
        /// <param name="eventCount">Eventos disponibles actualmente.</param>
        /// <param name="lastUpdate">Última actualización de datos informada por SharpAPI.</param>
        /// <param name="requiresTier">Nivel mínimo requerido (free, hobby, pro o sharp).</param>
        /// <param name="comingSoon">Indica si el libro fue anunciado pero aún no está disponible.</param>
        public Sportsbook(
            string id,
            int? numericalId,
            string displayName,
            string shortName,
            string category,
            bool hasLiveOdds,
            bool hasPlayerProps,
            bool? hasLimits,
            IEnumerable<string> regions,
            bool isSharp,
            string status,
            int eventCount,
            DateTime? lastUpdate,
            string requiresTier,
            bool comingSoon)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
            ArgumentNullException.ThrowIfNull(regions);

            Id = id;
            NumericalId = numericalId;
            DisplayName = displayName;
            ShortName = string.IsNullOrWhiteSpace(shortName) ? displayName : shortName;
            Category = category;
            HasLiveOdds = hasLiveOdds;
            HasPlayerProps = hasPlayerProps;
            HasLimits = hasLimits;
            Regions = regions
                .Where(region => !string.IsNullOrWhiteSpace(region))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(region => new SportsbookRegion(id, region))
                .ToList();
            IsSharp = isSharp;
            Status = string.IsNullOrWhiteSpace(status) ? "inactive" : status;
            EventCount = eventCount;
            LastUpdate = lastUpdate;
            RequiresTier = string.IsNullOrWhiteSpace(requiresTier) ? "free" : requiresTier;
            ComingSoon = comingSoon;
        }

        /// <summary>
        /// Identificador único de la casa, usado en filtros de SharpAPI.
        /// </summary>
        public string Id { get; protected set; } = string.Empty;

        /// <summary>
        /// Clave numérica estable. Es opcional cuando SharpAPI aún no la ha asignado.
        /// </summary>
        public int? NumericalId { get; protected set; }

        /// <summary>
        /// Nombre legible de la casa de apuestas.
        /// </summary>
        public string DisplayName { get; protected set; } = string.Empty;

        /// <summary>
        /// Nombre corto para mostrar.
        /// </summary>
        public string ShortName { get; protected set; } = string.Empty;

        /// <summary>
        /// Categoría del libro (major_us, regional, sharp, etc.).
        /// </summary>
        public string Category { get; protected set; } = string.Empty;

        /// <summary>
        /// Indica si admite apuestas en directo.
        /// </summary>
        public bool HasLiveOdds { get; protected set; }

        /// <summary>
        /// Indica si admite mercados de apuestas de jugadores.
        /// </summary>
        public bool HasPlayerProps { get; protected set; }

        /// <summary>
        /// Indica si publica límites de apuesta; nulo cuando no se han observado filas.
        /// </summary>
        public bool? HasLimits { get; protected set; }

        /// <summary>
        /// Regiones en las que opera la casa.
        /// </summary>
        public ICollection<SportsbookRegion> Regions { get; protected set; } = new List<SportsbookRegion>();

        /// <summary>
        /// Indica si el libro se considera un mercado sharp o eficiente.
        /// </summary>
        public bool IsSharp { get; protected set; }

        /// <summary>
        /// Estado de la casa: active o inactive.
        /// </summary>
        public string Status { get; protected set; } = string.Empty;

        /// <summary>
        /// Número de eventos disponibles actualmente.
        /// </summary>
        public int EventCount { get; protected set; }

        /// <summary>
        /// Marca de tiempo de la última actualización de datos del libro.
        /// </summary>
        public DateTime? LastUpdate { get; protected set; }

        /// <summary>
        /// Nivel mínimo de suscripción requerido.
        /// </summary>
        public string RequiresTier { get; protected set; } = string.Empty;

        /// <summary>
        /// Indica si el libro está anunciado y todavía no disponible.
        /// </summary>
        public bool ComingSoon { get; protected set; }
    }
}
