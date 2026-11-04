namespace Domain.Entities
{
    /// <summary>
    /// Tipo de mercado del catálogo de SharpAPI.
    /// </summary>
    public class Market
    {
        /// <summary>
        /// Constructor requerido por el mapeo de persistencia.
        /// </summary>
        protected Market()
        {
        }

        /// <summary>
        /// Crea un mercado de dominio a partir de los datos del catálogo.
        /// </summary>
        /// <param name="id">Slug del mercado, usado como filtro market_type o market.</param>
        /// <param name="name">Etiqueta visible; no se usa como filtro.</param>
        /// <param name="numericalId">Identificador numérico estable; puede no existir.</param>
        /// <param name="eventCount">Cantidad de eventos que ofrecen este mercado.</param>
        /// <param name="sportIds">Identificadores de deportes que actualmente tienen este mercado.</param>
        public Market(
            string id,
            string name,
            int? numericalId,
            int eventCount,
            IEnumerable<string> sportIds)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(sportIds);

            Id = id;
            Name = name;
            NumericalId = numericalId;
            EventCount = eventCount;
            Sports = sportIds
                .Where(sportId => !string.IsNullOrWhiteSpace(sportId))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(sportId => new MarketSport(id, sportId))
                .ToList();
        }

        /// <summary>
        /// Slug del mercado, valor exacto para filtros de cuotas.
        /// </summary>
        public string Id { get; protected set; } = string.Empty;

        /// <summary>
        /// Nombre legible del mercado, solo para visualización.
        /// </summary>
        public string Name { get; protected set; } = string.Empty;

        /// <summary>
        /// Clave numérica estable. Es opcional cuando SharpAPI aún no la ha asignado.
        /// </summary>
        public int? NumericalId { get; protected set; }

        /// <summary>
        /// Número de eventos que actualmente ofrecen este mercado.
        /// </summary>
        public int EventCount { get; protected set; }

        /// <summary>
        /// Deportes que actualmente tienen este mercado.
        /// </summary>
        public ICollection<MarketSport> Sports { get; protected set; } = new List<MarketSport>();
    }
}
