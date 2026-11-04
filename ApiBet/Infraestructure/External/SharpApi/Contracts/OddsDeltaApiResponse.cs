using System.Text.Json.Serialization;

namespace Infraestructure.External.SharpApi.Contracts
{
    /// <summary>
    /// Contrato JSON del endpoint GET /odds/delta de SharpAPI.
    /// </summary>
    internal sealed class OddsDeltaApiResponse
    {
        /// <summary>
        /// Cuotas activas o actualizadas.
        /// </summary>
        [JsonPropertyName("data")]
        public List<OddsApiItem> Data { get; set; } = [];

        /// <summary>
        /// Identificadores de cuotas retiradas.
        /// </summary>
        [JsonPropertyName("removed")]
        public List<OddsDeltaRemovedApiItem> Removed { get; set; } = [];

        /// <summary>
        /// Metadatos de paginación.
        /// </summary>
        [JsonPropertyName("pagination")]
        public OddsPaginationApi? Pagination { get; set; }

        /// <summary>
        /// Marca de tiempo de actualización del catálogo remoto.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }

        /// <summary>
        /// Indica si SharpAPI recortó el lote por desbordamiento.
        /// </summary>
        [JsonPropertyName("overflow")]
        public bool Overflow { get; set; }

        /// <summary>
        /// Indica si la lista de retirados llegó truncada.
        /// </summary>
        [JsonPropertyName("removed_truncated")]
        public bool RemovedTruncated { get; set; }

        /// <summary>
        /// Indica si SharpAPI ajustó el parámetro since.
        /// </summary>
        [JsonPropertyName("since_clamped")]
        public bool SinceClamped { get; set; }
    }

    /// <summary>
    /// Elemento retirado del feed GET /odds/delta.
    /// </summary>
    internal sealed class OddsDeltaRemovedApiItem
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("sportsbook")]
        public string? Sportsbook { get; set; }

        [JsonPropertyName("removed_at")]
        public string? RemovedAt { get; set; }

        [JsonPropertyName("event_start")]
        public string? EventStart { get; set; }

        [JsonPropertyName("was_live")]
        public bool? WasLive { get; set; }

        [JsonPropertyName("boundary")]
        public bool? Boundary { get; set; }
    }
}
