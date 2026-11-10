namespace Aplication.Configuration
{
    /// <summary>
    /// Opciones de sincronización de GET /odds/best. Solo se configuran en OddsSync de appsettings.json.
    /// </summary>
    public sealed class OddsSyncOptions
    {
        /// <summary>
        /// Nombre de la sección en appsettings.
        /// </summary>
        public const string SectionName = "OddsSync";

        /// <summary>
        /// Tamaño de página enviado a SharpAPI. Máximo 200.
        /// </summary>
        public int Limit { get; set; } = 200;

        /// <summary>
        /// Offset inicial de la primera iteración.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Offset máximo inclusive; el ciclo se detiene al alcanzarlo.
        /// </summary>
        public int MaxOffset { get; set; } = 20000;

        /// <summary>
        /// Incremento del offset en cada iteración.
        /// </summary>
        public int OffsetIncrement { get; set; } = 200;

        /// <summary>
        /// Segundos de espera entre iteraciones.
        /// </summary>
        public int DelaySeconds { get; set; } = 1;

        /// <summary>
        /// Deportes a consultar, separados por coma.
        /// </summary>
        public string Sports { get; set; } = "Baseball,Basketball,Darts,Lacrosse,Table Tennis,Tennis,Volleyball";

        /// <summary>
        /// Filtro SharpAPI market. Vacío o nulo para no enviarlo.
        /// </summary>
        public string? Market { get; set; }

        /// <summary>
        /// Filtro SharpAPI is_main_line. Nulo para no enviarlo.
        /// </summary>
        public bool? IsMainLine { get; set; } = true;

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
        /// Obtiene el filtro market, o nulo si no debe enviarse.
        /// </summary>
        public string? GetMarketFilter()
        {
            return string.IsNullOrWhiteSpace(Market) ? null : Market.Trim();
        }

        /// <summary>
        /// Divide un CSV y normaliza espacios a guion bajo.
        /// </summary>
        private static IReadOnlyList<string> SplitCsv(string? value)
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
