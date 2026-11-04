using System.Text.Json.Serialization;

namespace Infraestructure.External.SharpApi.Contracts
{
    /// <summary>
    /// Contrato JSON del endpoint GET /sports de SharpAPI.
    /// </summary>
    internal sealed class SportsApiResponse
    {
        /// <summary>
        /// Lista de deportes reportados por el proveedor.
        /// </summary>
        [JsonPropertyName("data")]
        public List<SportApiItem> Data { get; set; } = [];

        /// <summary>
        /// Marca de tiempo de actualización del catálogo remoto.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Elemento de deporte dentro de la respuesta de SharpAPI.
    /// </summary>
    internal sealed class SportApiItem
    {
        /// <summary>
        /// Identificador textual del deporte.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Nombre visible del deporte.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Identificador numérico estable; puede omitirse.
        /// </summary>
        [JsonPropertyName("numerical_id")]
        public int? NumericalId { get; set; }

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

        /// <summary>
        /// Códigos de ligas asociadas al deporte.
        /// </summary>
        [JsonPropertyName("leagues")]
        public List<string> Leagues { get; set; } = [];
    }
}
