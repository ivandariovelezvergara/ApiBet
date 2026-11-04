namespace Aplication.Configuration
{
    /// <summary>
    /// Opciones de sincronización de GET /opportunities/arbitrage de SharpAPI.
    /// </summary>
    public sealed class ArbitrageFeedOptions
    {
        /// <summary>
        /// Nombre de la sección en appsettings.
        /// </summary>
        public const string SectionName = "ArbitrageFeed";

        /// <summary>
        /// Tamaño de página enviado a SharpAPI. Máximo 200.
        /// </summary>
        public int Limit { get; set; } = 50;

        /// <summary>
        /// Offset inicial de la primera página.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Máximo de páginas a recorrer para evitar un ciclo infinito.
        /// </summary>
        public int MaxPages { get; set; } = 20;

        /// <summary>
        /// Segundos de espera entre páginas.
        /// </summary>
        public int DelaySeconds { get; set; } = 2;

        /// <summary>
        /// Deportes a consultar, separados por coma. Vacío para no enviar el filtro sport.
        /// </summary>
        public string Sports { get; set; } = string.Empty;

        /// <summary>
        /// Ligas a consultar, separadas por coma. Vacío para no enviarlo.
        /// </summary>
        public string League { get; set; } = string.Empty;

        /// <summary>
        /// Casas a consultar, separadas por coma. Vacío para no enviarlo.
        /// </summary>
        public string Sportsbook { get; set; } = "bwin,betsson,stake";

        /// <summary>
        /// Tipos de mercado, separados por coma. Vacío para no enviarlo.
        /// </summary>
        public string Market { get; set; } = string.Empty;

        /// <summary>
        /// Beneficio mínimo en porcentaje (0.5 = 0.5 %). Nulo para omitir el parámetro.
        /// </summary>
        public decimal? MinProfit { get; set; } = 0.5m;

        /// <summary>
        /// Filtro SharpAPI is_live. Nulo para prepartido y en vivo.
        /// </summary>
        public bool? IsLive { get; set; }

        /// <summary>
        /// Código de estado EE. UU. para deep links. Vacío para el default de SharpAPI (pa).
        /// </summary>
        public string State { get; set; } = "pa";

        /// <summary>
        /// Minutos de vida mínima. El sync no borra filas más jóvenes; a los 3 minutos exactos o más sí.
        /// </summary>
        public int RetentionMinutes { get; set; } = 3;

        /// <summary>
        /// Obtiene los slugs de deporte normalizados para SharpAPI.
        /// </summary>
        public IReadOnlyList<string> GetSportSlugs()
        {
            return SplitCsv(Sports);
        }

        /// <summary>
        /// Obtiene el filtro sport ya unido, o nulo si está vacío.
        /// </summary>
        public string? GetSportFilter()
        {
            var slugs = GetSportSlugs();
            return slugs.Count == 0 ? null : string.Join(',', slugs);
        }

        /// <summary>
        /// Obtiene el filtro league ya unido, o nulo si está vacío.
        /// </summary>
        public string? GetLeagueFilter()
        {
            var values = SplitCsv(League);
            return values.Count == 0 ? null : string.Join(',', values);
        }

        /// <summary>
        /// Obtiene el filtro sportsbook ya unido, o nulo si está vacío.
        /// </summary>
        public string? GetSportsbookFilter()
        {
            var values = SplitCsv(Sportsbook);
            return values.Count == 0 ? null : string.Join(',', values);
        }

        /// <summary>
        /// Obtiene el filtro market ya unido, o nulo si está vacío.
        /// </summary>
        public string? GetMarketFilter()
        {
            var values = SplitCsv(Market);
            return values.Count == 0 ? null : string.Join(',', values);
        }

        /// <summary>
        /// Divide un CSV y normaliza espacios a guion bajo.
        /// </summary>
        private static IReadOnlyList<string> SplitCsv(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return [];
            }

            return value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(item => item.ToLowerInvariant().Replace(' ', '_'))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }
}
