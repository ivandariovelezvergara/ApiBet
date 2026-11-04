using System.Text.Json.Serialization;

namespace Infraestructure.External.SharpApi.Contracts
{
    /// <summary>
    /// Contrato JSON del endpoint GET /sportsbooks de SharpAPI.
    /// </summary>
    internal sealed class SportsbooksApiResponse
    {
        /// <summary>
        /// Lista de casas de apuestas reportadas por el proveedor.
        /// </summary>
        [JsonPropertyName("data")]
        public List<SportsbookApiItem> Data { get; set; } = [];

        /// <summary>
        /// Marca de tiempo de actualización del catálogo remoto.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Elemento de casa de apuestas dentro de la respuesta de SharpAPI.
    /// </summary>
    internal sealed class SportsbookApiItem
    {
        /// <summary>
        /// Identificador único de la casa.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Identificador numérico estable; puede omitirse.
        /// </summary>
        [JsonPropertyName("numerical_id")]
        public int? NumericalId { get; set; }

        /// <summary>
        /// Nombre legible de la casa.
        /// </summary>
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Nombre corto para mostrar.
        /// </summary>
        [JsonPropertyName("short_name")]
        public string ShortName { get; set; } = string.Empty;

        /// <summary>
        /// Categoría del libro.
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Indica si admite apuestas en directo.
        /// </summary>
        [JsonPropertyName("has_live_odds")]
        public bool HasLiveOdds { get; set; }

        /// <summary>
        /// Indica si admite mercados de jugador.
        /// </summary>
        [JsonPropertyName("has_player_props")]
        public bool HasPlayerProps { get; set; }

        /// <summary>
        /// Indica si publica límites; nulo si no se ha observado.
        /// </summary>
        [JsonPropertyName("has_limits")]
        public bool? HasLimits { get; set; }

        /// <summary>
        /// Regiones disponibles.
        /// </summary>
        [JsonPropertyName("regions")]
        public List<string> Regions { get; set; } = [];

        /// <summary>
        /// Indica si se considera un mercado sharp.
        /// </summary>
        [JsonPropertyName("is_sharp")]
        public bool IsSharp { get; set; }

        /// <summary>
        /// Estado operativo (active o inactive).
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Eventos disponibles actualmente.
        /// </summary>
        [JsonPropertyName("event_count")]
        public int EventCount { get; set; }

        /// <summary>
        /// Última actualización de datos en formato ISO-8601.
        /// </summary>
        [JsonPropertyName("last_update")]
        public string? LastUpdate { get; set; }

        /// <summary>
        /// Nivel mínimo requerido.
        /// </summary>
        [JsonPropertyName("requires_tier")]
        public string RequiresTier { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el libro está anunciado y aún no disponible.
        /// </summary>
        [JsonPropertyName("coming_soon")]
        public bool ComingSoon { get; set; }
    }
}
