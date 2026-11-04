using System.Text.Json.Serialization;

namespace Infraestructure.External.SharpApi.Contracts
{
    /// <summary>
    /// Contrato JSON del endpoint GET /opportunities/arbitrage de SharpAPI.
    /// </summary>
    internal sealed class ArbitrageApiResponse
    {
        /// <summary>
        /// Oportunidades de la página.
        /// </summary>
        [JsonPropertyName("data")]
        public List<ArbitrageApiItem> Data { get; set; } = [];

        /// <summary>
        /// Metadatos de paginación.
        /// </summary>
        [JsonPropertyName("pagination")]
        public ArbitragePaginationApi? Pagination { get; set; }

        /// <summary>
        /// Marca de tiempo de actualización del catálogo remoto.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Paginación informada por SharpAPI para GET /opportunities/arbitrage.
    /// </summary>
    internal sealed class ArbitragePaginationApi
    {
        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        [JsonPropertyName("offset")]
        public int? Offset { get; set; }

        [JsonPropertyName("count")]
        public int? Count { get; set; }

        [JsonPropertyName("total")]
        public int? Total { get; set; }

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("next_offset")]
        public int? NextOffset { get; set; }
    }

    /// <summary>
    /// Elemento de oportunidad de arbitraje.
    /// </summary>
    internal sealed class ArbitrageApiItem
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("detected_at")]
        public string? DetectedAt { get; set; }

        [JsonPropertyName("estimated_net_profit_percent")]
        public decimal? EstimatedNetProfitPercent { get; set; }

        [JsonPropertyName("ev_available")]
        public bool EvAvailable { get; set; }

        [JsonPropertyName("ev_percentage")]
        public decimal? EvPercentage { get; set; }

        [JsonPropertyName("event_id")]
        public string EventId { get; set; } = string.Empty;

        [JsonPropertyName("event_name")]
        public string EventName { get; set; } = string.Empty;

        [JsonPropertyName("implied_total")]
        public decimal? ImpliedTotal { get; set; }

        [JsonPropertyName("is_alternate_line")]
        public bool IsAlternateLine { get; set; }

        [JsonPropertyName("is_live")]
        public bool IsLive { get; set; }

        [JsonPropertyName("is_player_prop")]
        public bool IsPlayerProp { get; set; }

        [JsonPropertyName("league")]
        public string League { get; set; } = string.Empty;

        [JsonPropertyName("league_label")]
        public string? LeagueLabel { get; set; }

        [JsonPropertyName("line")]
        public decimal? Line { get; set; }

        [JsonPropertyName("line_unchanged")]
        public bool LineUnchanged { get; set; }

        [JsonPropertyName("market_label")]
        public string? MarketLabel { get; set; }

        [JsonPropertyName("market_type")]
        public string MarketType { get; set; } = string.Empty;

        [JsonPropertyName("oldest_odds_age_seconds")]
        public decimal? OldestOddsAgeSeconds { get; set; }

        [JsonPropertyName("player_name")]
        public string? PlayerName { get; set; }

        [JsonPropertyName("possibly_stale")]
        public bool PossiblyStale { get; set; }

        [JsonPropertyName("profit_percent")]
        public decimal? ProfitPercent { get; set; }

        [JsonPropertyName("sport")]
        public string Sport { get; set; } = string.Empty;

        [JsonPropertyName("start_time")]
        public string? StartTime { get; set; }

        [JsonPropertyName("stat_category")]
        public string? StatCategory { get; set; }

        [JsonPropertyName("warnings")]
        public List<string> Warnings { get; set; } = [];

        [JsonPropertyName("legs")]
        public List<ArbitrageLegApiItem> Legs { get; set; } = [];
    }

    /// <summary>
    /// Pata de una oportunidad de arbitraje.
    /// </summary>
    internal sealed class ArbitrageLegApiItem
    {
        [JsonPropertyName("deep_link")]
        public string? DeepLink { get; set; }

        [JsonPropertyName("external_event_id")]
        public string? ExternalEventId { get; set; }

        [JsonPropertyName("external_market_id")]
        public string? ExternalMarketId { get; set; }

        [JsonPropertyName("line")]
        public decimal? Line { get; set; }

        [JsonPropertyName("market_id")]
        public string? MarketId { get; set; }

        [JsonPropertyName("odds_american")]
        public decimal? OddsAmerican { get; set; }

        [JsonPropertyName("odds_decimal")]
        public decimal? OddsDecimal { get; set; }

        [JsonPropertyName("odds_id")]
        public string? OddsId { get; set; }

        [JsonPropertyName("odds_probability")]
        public decimal? OddsProbability { get; set; }

        [JsonPropertyName("selection")]
        public string Selection { get; set; } = string.Empty;

        [JsonPropertyName("selection_id")]
        public string? SelectionId { get; set; }

        [JsonPropertyName("sportsbook")]
        public string Sportsbook { get; set; } = string.Empty;

        [JsonPropertyName("stake_percent")]
        public decimal? StakePercent { get; set; }
    }
}
