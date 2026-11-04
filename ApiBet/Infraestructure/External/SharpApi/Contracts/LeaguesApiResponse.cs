using System.Text.Json.Serialization;

namespace Infraestructure.External.SharpApi.Contracts
{
    /// <summary>
    /// Contrato JSON del endpoint GET /leagues de SharpAPI.
    /// </summary>
    internal sealed class LeaguesApiResponse
    {
        /// <summary>
        /// Lista de ligas reportadas por el proveedor.
        /// </summary>
        [JsonPropertyName("data")]
        public List<LeagueApiItem> Data { get; set; } = [];

        /// <summary>
        /// Marca de tiempo de actualización del catálogo remoto.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Elemento de liga dentro de la respuesta de SharpAPI.
    /// </summary>
    internal sealed class LeagueApiItem
    {
        /// <summary>
        /// Identificador textual de la liga.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Nombre visible de la liga.
        /// </summary>
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Identificador numérico estable; puede omitirse.
        /// </summary>
        [JsonPropertyName("numerical_id")]
        public int? NumericalId { get; set; }

        /// <summary>
        /// Identificador del deporte al que pertenece.
        /// </summary>
        [JsonPropertyName("sport")]
        public string Sport { get; set; } = string.Empty;

        /// <summary>
        /// Total de eventos con cuotas.
        /// </summary>
        [JsonPropertyName("event_count")]
        public int EventCount { get; set; }

        /// <summary>
        /// Eventos actualmente en vivo.
        /// </summary>
        [JsonPropertyName("live_count")]
        public int LiveCount { get; set; }
    }
}
