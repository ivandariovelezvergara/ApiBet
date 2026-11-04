using System.Text.Json;

namespace Infraestructure.Serialization
{
    /// <summary>
    /// Opciones JSON de carga masiva hacia SQL Server.
    /// </summary>
    internal static class BulkJson
    {
        /// <summary>
        /// CamelCase y fechas en reloj local sin zona horaria.
        /// </summary>
        public static JsonSerializerOptions Options { get; } = Create();

        /// <summary>
        /// Crea las opciones compartidas de serialización masiva.
        /// </summary>
        private static JsonSerializerOptions Create()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            options.Converters.Add(new LocalClockDateTimeJsonConverter());
            options.Converters.Add(new NullableLocalClockDateTimeJsonConverter());
            return options;
        }
    }
}
