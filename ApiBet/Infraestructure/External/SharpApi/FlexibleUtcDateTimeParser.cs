using System.Globalization;

namespace Infraestructure.External.SharpApi
{
    /// <summary>
    /// Interpreta fechas ISO-8601 de SharpAPI, incluyendo fracciones de segundo extra.
    /// </summary>
    internal static class FlexibleUtcDateTimeParser
    {
        /// <summary>
        /// Convierte un texto ISO-8601 a hora local de la máquina. Devuelve nulo si el valor está vacío o es inválido.
        /// </summary>
        /// <param name="value">Fecha original enviada por SharpAPI.</param>
        /// <returns>Fecha local o nulo cuando no se puede interpretar.</returns>
        public static DateTime? Parse(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (DateTimeOffset.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out var parsed))
            {
                return DateTime.SpecifyKind(parsed.UtcDateTime.ToLocalTime(), DateTimeKind.Unspecified);
            }

            return null;
        }
    }
}
