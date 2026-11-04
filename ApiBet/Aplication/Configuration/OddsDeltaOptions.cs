namespace Aplication.Configuration
{
    /// <summary>
    /// Opciones de sincronización de GET /odds/delta. Solo se configuran en OddsDelta de appsettings.json.
    /// </summary>
    public sealed class OddsDeltaOptions
    {
        /// <summary>
        /// Nombre de la sección en appsettings.
        /// </summary>
        public const string SectionName = "OddsDelta";

        /// <summary>
        /// Tamaño de página enviado a SharpAPI. Máximo 500.
        /// </summary>
        public int Limit { get; set; } = 500;

        /// <summary>
        /// Offset inicial de la primera página. Se omite en páginas siguientes cuando hay cursor.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Máximo de páginas a recorrer para evitar un ciclo infinito.
        /// </summary>
        public int MaxPages { get; set; } = 100;

        /// <summary>
        /// Segundos de espera entre páginas.
        /// </summary>
        public int DelaySeconds { get; set; } = 6;

        /// <summary>
        /// Deportes a consultar, separados por coma. Vacío para no enviar el filtro sport.
        /// </summary>
        public string Sports { get; set; } = string.Empty;

        /// <summary>
        /// Filtro SharpAPI market, separados por coma. Vacío o nulo para no enviarlo.
        /// </summary>
        public string? Market { get; set; }

        /// <summary>
        /// Filtro SharpAPI is_live. Nulo para no enviarlo.
        /// </summary>
        public bool? IsLive { get; set; }

        /// <summary>
        /// Filtro SharpAPI is_main_line. Nulo para no enviarlo.
        /// </summary>
        public bool? IsMainLine { get; set; }

        /// <summary>
        /// Obtiene el since UTC automático: inicio del día UTC actual (yyyy-MM-ddTHH:mm:ssZ).
        /// </summary>
        public string GetSinceUtc()
        {
            return DateTime.UtcNow.Date.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

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
