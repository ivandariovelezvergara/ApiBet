using System.Text.Json.Serialization;

namespace Infraestructure.External.SharpApi.Contracts
{
    /// <summary>
    /// Contrato JSON del endpoint GET /odds/best de SharpAPI.
    /// </summary>
    internal sealed class BestOddsApiResponse
    {
        /// <summary>
        /// Filas de mejores cuotas.
        /// </summary>
        [JsonPropertyName("data")]
        public List<BestOddApiItem> Data { get; set; } = [];

        /// <summary>
        /// Metadatos de paginación.
        /// </summary>
        [JsonPropertyName("pagination")]
        public BestOddsPaginationApi? Pagination { get; set; }

        /// <summary>
        /// Marca de tiempo de actualización del catálogo remoto.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Paginación informada por SharpAPI.
    /// </summary>
    internal sealed class BestOddsPaginationApi
    {
        /// <summary>
        /// Indica si hay más páginas.
        /// </summary>
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        /// <summary>
        /// Siguiente offset sugerido.
        /// </summary>
        [JsonPropertyName("next_offset")]
        public int? NextOffset { get; set; }
    }

    /// <summary>
    /// Elemento de mejores cuotas.
    /// </summary>
    internal sealed class BestOddApiItem
    {
        [JsonPropertyName("event_id")]
        public string EventId { get; set; } = string.Empty;

        [JsonPropertyName("event_name")]
        public string EventName { get; set; } = string.Empty;

        [JsonPropertyName("sport")]
        public string Sport { get; set; } = string.Empty;

        [JsonPropertyName("league")]
        public string League { get; set; } = string.Empty;

        [JsonPropertyName("market_type")]
        public string MarketType { get; set; } = string.Empty;

        [JsonPropertyName("selection")]
        public string Selection { get; set; } = string.Empty;

        [JsonPropertyName("line")]
        public decimal? Line { get; set; }

        [JsonPropertyName("is_main_line")]
        public bool IsMainLine { get; set; }

        [JsonPropertyName("best_odds")]
        public OddsQuoteApi? BestOdds { get; set; }

        [JsonPropertyName("consensus_odds")]
        public OddsQuoteApi? ConsensusOdds { get; set; }

        [JsonPropertyName("market_hold")]
        public decimal? MarketHold { get; set; }

        [JsonPropertyName("best_book")]
        public string? BestBook { get; set; }

        [JsonPropertyName("all_books")]
        public List<BestOddBookApiItem> AllBooks { get; set; } = [];

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("player_name")]
        public string? PlayerName { get; set; }

        [JsonPropertyName("stat_category")]
        public string? StatCategory { get; set; }
    }

    /// <summary>
    /// Bloque de cuota americana/decimal/probabilidad.
    /// </summary>
    internal sealed class OddsQuoteApi
    {
        [JsonPropertyName("american")]
        public decimal? American { get; set; }

        [JsonPropertyName("decimal")]
        public decimal? Decimal { get; set; }

        [JsonPropertyName("probability")]
        public decimal? Probability { get; set; }
    }

    /// <summary>
    /// Cuota de una casa dentro de all_books.
    /// </summary>
    internal sealed class BestOddBookApiItem
    {
        [JsonPropertyName("book")]
        public string? Book { get; set; }

        [JsonPropertyName("sportsbook")]
        public string? Sportsbook { get; set; }

        [JsonPropertyName("odds")]
        public OddsQuoteApi? Odds { get; set; }

        [JsonPropertyName("edge")]
        public decimal? Edge { get; set; }
    }
}
