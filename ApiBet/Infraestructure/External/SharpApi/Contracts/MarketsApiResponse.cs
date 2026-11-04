using System.Text.Json.Serialization;

namespace Infraestructure.External.SharpApi.Contracts
{
    /// <summary>
    /// Contrato JSON del endpoint GET /markets de SharpAPI.
    /// </summary>
    internal sealed class MarketsApiResponse
    {
        /// <summary>
        /// Lista de mercados reportados por el proveedor.
        /// </summary>
        [JsonPropertyName("data")]
        public List<MarketApiItem> Data { get; set; } = [];

        /// <summary>
        /// Marca de tiempo de actualización del catálogo remoto.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Elemento de mercado dentro de la respuesta de SharpAPI.
    /// </summary>
    internal sealed class MarketApiItem
    {
        /// <summary>
        /// Slug del mercado, usado como filtro.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Etiqueta visible del mercado.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Identificador numérico estable; puede omitirse.
        /// </summary>
        [JsonPropertyName("numerical_id")]
        public int? NumericalId { get; set; }

        /// <summary>
        /// Eventos que actualmente ofrecen este mercado.
        /// </summary>
        [JsonPropertyName("event_count")]
        public int EventCount { get; set; }

        /// <summary>
        /// Identificadores de deportes que tienen este mercado.
        /// </summary>
        [JsonPropertyName("sports")]
        public List<string> Sports { get; set; } = [];
    }
}
