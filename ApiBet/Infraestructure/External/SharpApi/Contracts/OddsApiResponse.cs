using System.Text.Json.Serialization;

namespace Infraestructure.External.SharpApi.Contracts
{
    /// <summary>
    /// Contrato JSON del endpoint GET /odds de SharpAPI.
    /// </summary>
    internal sealed class OddsApiResponse
    {
        /// <summary>
        /// Filas de cuotas.
        /// </summary>
        [JsonPropertyName("data")]
        public List<OddsApiItem> Data { get; set; } = [];

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
    }

    /// <summary>
    /// Paginación informada por SharpAPI para GET /odds.
    /// </summary>
    internal sealed class OddsPaginationApi
    {
        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        [JsonPropertyName("offset")]
        public int? Offset { get; set; }

        [JsonPropertyName("count")]
        public int? Count { get; set; }

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("next_offset")]
        public int? NextOffset { get; set; }

        /// <summary>
        /// Cursor opaco para paginación profunda; tiene prioridad sobre offset.
        /// </summary>
        [JsonPropertyName("next_cursor")]
        public string? NextCursor { get; set; }
    }

    /// <summary>
    /// Elemento de cuota del feed GET /odds.
    /// </summary>
    internal sealed class OddsApiItem
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("sportsbook")]
        public string Sportsbook { get; set; } = string.Empty;

        [JsonPropertyName("event_id")]
        public string EventId { get; set; } = string.Empty;

        [JsonPropertyName("event_uuid")]
        public string? EventUuid { get; set; }

        [JsonPropertyName("sport")]
        public string Sport { get; set; } = string.Empty;

        [JsonPropertyName("league")]
        public string League { get; set; } = string.Empty;

        [JsonPropertyName("home_team")]
        public string? HomeTeam { get; set; }

        [JsonPropertyName("away_team")]
        public string? AwayTeam { get; set; }

        [JsonPropertyName("market_type")]
        public string MarketType { get; set; } = string.Empty;

        [JsonPropertyName("selection")]
        public string Selection { get; set; } = string.Empty;

        [JsonPropertyName("selection_type")]
        public string? SelectionType { get; set; }

        [JsonPropertyName("team_side")]
        public string? TeamSide { get; set; }

        [JsonPropertyName("odds_american")]
        public decimal? OddsAmerican { get; set; }

        [JsonPropertyName("odds_decimal")]
        public decimal? OddsDecimal { get; set; }

        [JsonPropertyName("odds_probability")]
        public decimal? OddsProbability { get; set; }

        [JsonPropertyName("line")]
        public decimal? Line { get; set; }

        [JsonPropertyName("event_start_time")]
        public string? EventStartTime { get; set; }

        [JsonPropertyName("is_live")]
        public bool IsLive { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; } = true;

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("deep_link")]
        public string? DeepLink { get; set; }

        [JsonPropertyName("market_id")]
        public string? MarketId { get; set; }

        [JsonPropertyName("selection_id")]
        public string? SelectionId { get; set; }

        [JsonPropertyName("external_market_id")]
        public string? ExternalMarketId { get; set; }

        [JsonPropertyName("is_alternate_line")]
        public bool IsAlternateLine { get; set; }

        [JsonPropertyName("is_main_line")]
        public bool IsMainLine { get; set; }

        [JsonPropertyName("is_stale_pregame_price")]
        public bool IsStalePregamePrice { get; set; }

        [JsonPropertyName("is_player_prop")]
        public bool IsPlayerProp { get; set; }

        [JsonPropertyName("is_impossible_scoreline")]
        public bool IsImpossibleScoreline { get; set; }

        [JsonPropertyName("season_type")]
        public string? SeasonType { get; set; }

        [JsonPropertyName("home")]
        public OddsTeamRefApi? Home { get; set; }

        [JsonPropertyName("away")]
        public OddsTeamRefApi? Away { get; set; }

        [JsonPropertyName("sport_ref")]
        public OddsNamedRefApi? SportRef { get; set; }

        [JsonPropertyName("league_ref")]
        public OddsLabeledRefApi? LeagueRef { get; set; }

        [JsonPropertyName("market_ref")]
        public OddsLabeledRefApi? MarketRef { get; set; }

        [JsonPropertyName("sportsbook_ref")]
        public OddsLabeledRefApi? SportsbookRef { get; set; }

        [JsonPropertyName("competition_ref")]
        public OddsLabeledRefApi? CompetitionRef { get; set; }

        [JsonPropertyName("external_event_id")]
        public string? ExternalEventId { get; set; }
    }

    /// <summary>
    /// Referencia de equipo u jugador anidada en una cuota.
    /// </summary>
    internal sealed class OddsTeamRefApi
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("numerical_id")]
        public int? NumericalId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("logo")]
        public string? Logo { get; set; }

        [JsonPropertyName("abbreviation")]
        public string? Abbreviation { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("mascot")]
        public string? Mascot { get; set; }

        [JsonPropertyName("conference")]
        public string? Conference { get; set; }

        [JsonPropertyName("division")]
        public string? Division { get; set; }
    }

    /// <summary>
    /// Referencia con name (deporte).
    /// </summary>
    internal sealed class OddsNamedRefApi
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("numerical_id")]
        public int? NumericalId { get; set; }
    }

    /// <summary>
    /// Referencia con label (liga, mercado, casa, competencia).
    /// </summary>
    internal sealed class OddsLabeledRefApi
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("numerical_id")]
        public int? NumericalId { get; set; }
    }
}
