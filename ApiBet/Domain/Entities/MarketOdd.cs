namespace Domain.Entities
{
    /// <summary>
    /// Cuota individual del endpoint GET /odds de SharpAPI.
    /// </summary>
    public class MarketOdd
    {
        /// <summary>
        /// Constructor requerido por el mapeo de persistencia.
        /// </summary>
        protected MarketOdd()
        {
        }

        /// <summary>
        /// Crea una fila de cuota de mercado a partir del catálogo remoto.
        /// </summary>
        public MarketOdd(
            string? id,
            string sportsbook,
            string eventId,
            string? eventUuid,
            string sport,
            string league,
            string? homeTeam,
            string? awayTeam,
            string marketType,
            string selection,
            string? selectionType,
            string? teamSide,
            decimal? oddsAmerican,
            decimal? oddsDecimal,
            decimal? oddsProbability,
            decimal? line,
            DateTime? eventStartTime,
            bool isLive,
            bool isActive,
            DateTime? timestamp,
            string? deepLink,
            string? marketId,
            string? selectionId,
            string? externalMarketId,
            bool isAlternateLine,
            bool isMainLine,
            bool isStalePregamePrice,
            bool isPlayerProp,
            bool isImpossibleScoreline,
            string? seasonType,
            string? homeId,
            int? homeNumericalId,
            string? homeName,
            string? homeLogo,
            string? homeAbbreviation,
            string? homeCity,
            string? homeMascot,
            string? homeConference,
            string? homeDivision,
            string? awayId,
            int? awayNumericalId,
            string? awayName,
            string? awayLogo,
            string? awayAbbreviation,
            string? awayCity,
            string? awayMascot,
            string? awayConference,
            string? awayDivision,
            string? sportRefId,
            string? sportRefName,
            int? sportRefNumericalId,
            string? leagueRefId,
            string? leagueRefLabel,
            int? leagueRefNumericalId,
            string? marketRefId,
            string? marketRefLabel,
            int? marketRefNumericalId,
            string? sportsbookRefId,
            string? sportsbookRefLabel,
            int? sportsbookRefNumericalId,
            string? competitionRefId,
            string? competitionRefLabel,
            int? competitionRefNumericalId,
            string? externalEventId)
        {
            Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString("N") : id.Trim();
            Sportsbook = sportsbook ?? string.Empty;
            EventId = eventId ?? string.Empty;
            EventUuid = eventUuid;
            Sport = sport ?? string.Empty;
            League = league ?? string.Empty;
            HomeTeam = homeTeam;
            AwayTeam = awayTeam;
            MarketType = marketType ?? string.Empty;
            Selection = selection ?? string.Empty;
            SelectionType = selectionType;
            TeamSide = teamSide;
            OddsAmerican = oddsAmerican;
            OddsDecimal = oddsDecimal;
            OddsProbability = oddsProbability;
            Line = line;
            EventStartTime = eventStartTime;
            IsLive = isLive;
            IsActive = isActive;
            Timestamp = timestamp;
            DeepLink = deepLink;
            MarketId = marketId;
            SelectionId = selectionId;
            ExternalMarketId = externalMarketId;
            IsAlternateLine = isAlternateLine;
            IsMainLine = isMainLine;
            IsStalePregamePrice = isStalePregamePrice;
            IsPlayerProp = isPlayerProp;
            IsImpossibleScoreline = isImpossibleScoreline;
            SeasonType = seasonType;
            HomeId = homeId;
            HomeNumericalId = homeNumericalId;
            HomeName = homeName;
            HomeLogo = homeLogo;
            HomeAbbreviation = homeAbbreviation;
            HomeCity = homeCity;
            HomeMascot = homeMascot;
            HomeConference = homeConference;
            HomeDivision = homeDivision;
            AwayId = awayId;
            AwayNumericalId = awayNumericalId;
            AwayName = awayName;
            AwayLogo = awayLogo;
            AwayAbbreviation = awayAbbreviation;
            AwayCity = awayCity;
            AwayMascot = awayMascot;
            AwayConference = awayConference;
            AwayDivision = awayDivision;
            SportRefId = sportRefId;
            SportRefName = sportRefName;
            SportRefNumericalId = sportRefNumericalId;
            LeagueRefId = leagueRefId;
            LeagueRefLabel = leagueRefLabel;
            LeagueRefNumericalId = leagueRefNumericalId;
            MarketRefId = marketRefId;
            MarketRefLabel = marketRefLabel;
            MarketRefNumericalId = marketRefNumericalId;
            SportsbookRefId = sportsbookRefId;
            SportsbookRefName = sportsbookRefLabel;
            SportsbookRefNumericalId = sportsbookRefNumericalId;
            CompetitionRefId = competitionRefId;
            CompetitionRefLabel = competitionRefLabel;
            CompetitionRefNumericalId = competitionRefNumericalId;
            ExternalEventId = externalEventId;
            CreatedAtUtc = DateTime.Now;
        }

        /// <summary>Identificador de la cuota en SharpAPI.</summary>
        public string Id { get; protected set; } = string.Empty;

        /// <summary>Casa de apuestas que publica la cuota.</summary>
        public string Sportsbook { get; protected set; } = string.Empty;

        /// <summary>Identificador del evento.</summary>
        public string EventId { get; protected set; } = string.Empty;

        /// <summary>UUID interno del evento en SharpAPI.</summary>
        public string? EventUuid { get; protected set; }

        /// <summary>Slug del deporte.</summary>
        public string Sport { get; protected set; } = string.Empty;

        /// <summary>Slug de la liga.</summary>
        public string League { get; protected set; } = string.Empty;

        /// <summary>Nombre del equipo o jugador local.</summary>
        public string? HomeTeam { get; protected set; }

        /// <summary>Nombre del equipo o jugador visitante.</summary>
        public string? AwayTeam { get; protected set; }

        /// <summary>Tipo de mercado (moneyline, moneyline_3-way, etc.).</summary>
        public string MarketType { get; protected set; } = string.Empty;

        /// <summary>Nombre de la selección.</summary>
        public string Selection { get; protected set; } = string.Empty;

        /// <summary>Tipo de selección (home, away, draw).</summary>
        public string? SelectionType { get; protected set; }

        /// <summary>Lado del equipo asociado a la selección.</summary>
        public string? TeamSide { get; protected set; }

        /// <summary>Cuota americana.</summary>
        public decimal? OddsAmerican { get; protected set; }

        /// <summary>Cuota decimal.</summary>
        public decimal? OddsDecimal { get; protected set; }

        /// <summary>Probabilidad implícita.</summary>
        public decimal? OddsProbability { get; protected set; }

        /// <summary>Línea asociada; nula en moneyline.</summary>
        public decimal? Line { get; protected set; }

        /// <summary>Inicio del evento en UTC.</summary>
        public DateTime? EventStartTime { get; protected set; }

        /// <summary>Indica si el evento está en vivo.</summary>
        public bool IsLive { get; protected set; }

        /// <summary>Indica si la cuota está activa.</summary>
        public bool IsActive { get; protected set; }

        /// <summary>Marca de tiempo de frescura informada por SharpAPI.</summary>
        public DateTime? Timestamp { get; protected set; }

        /// <summary>Enlace profundo de la cuota.</summary>
        public string? DeepLink { get; protected set; }

        /// <summary>Identificador del mercado en la casa.</summary>
        public string? MarketId { get; protected set; }

        /// <summary>Identificador de la selección en la casa.</summary>
        public string? SelectionId { get; protected set; }

        /// <summary>Identificador externo del mercado.</summary>
        public string? ExternalMarketId { get; protected set; }

        /// <summary>Indica si es una línea alternativa.</summary>
        public bool IsAlternateLine { get; protected set; }

        /// <summary>Indica si es la línea principal.</summary>
        public bool IsMainLine { get; protected set; }

        /// <summary>Indica si el precio pregame está desactualizado.</summary>
        public bool IsStalePregamePrice { get; protected set; }

        /// <summary>Indica si es un prop de jugador.</summary>
        public bool IsPlayerProp { get; protected set; }

        /// <summary>Indica si el marcador es imposible.</summary>
        public bool IsImpossibleScoreline { get; protected set; }

        /// <summary>Tipo de temporada informada por SharpAPI.</summary>
        public string? SeasonType { get; protected set; }

        /// <summary>Identificador del equipo local.</summary>
        public string? HomeId { get; protected set; }

        /// <summary>Identificador numérico del equipo local.</summary>
        public int? HomeNumericalId { get; protected set; }

        /// <summary>Nombre canónico del equipo local.</summary>
        public string? HomeName { get; protected set; }

        /// <summary>Logo del equipo local.</summary>
        public string? HomeLogo { get; protected set; }

        /// <summary>Abreviatura del equipo local.</summary>
        public string? HomeAbbreviation { get; protected set; }

        /// <summary>Ciudad del equipo local.</summary>
        public string? HomeCity { get; protected set; }

        /// <summary>Mascota del equipo local.</summary>
        public string? HomeMascot { get; protected set; }

        /// <summary>Conferencia del equipo local.</summary>
        public string? HomeConference { get; protected set; }

        /// <summary>División del equipo local.</summary>
        public string? HomeDivision { get; protected set; }

        /// <summary>Identificador del equipo visitante.</summary>
        public string? AwayId { get; protected set; }

        /// <summary>Identificador numérico del equipo visitante.</summary>
        public int? AwayNumericalId { get; protected set; }

        /// <summary>Nombre canónico del equipo visitante.</summary>
        public string? AwayName { get; protected set; }

        /// <summary>Logo del equipo visitante.</summary>
        public string? AwayLogo { get; protected set; }

        /// <summary>Abreviatura del equipo visitante.</summary>
        public string? AwayAbbreviation { get; protected set; }

        /// <summary>Ciudad del equipo visitante.</summary>
        public string? AwayCity { get; protected set; }

        /// <summary>Mascota del equipo visitante.</summary>
        public string? AwayMascot { get; protected set; }

        /// <summary>Conferencia del equipo visitante.</summary>
        public string? AwayConference { get; protected set; }

        /// <summary>División del equipo visitante.</summary>
        public string? AwayDivision { get; protected set; }

        /// <summary>Identificador de referencia del deporte.</summary>
        public string? SportRefId { get; protected set; }

        /// <summary>Nombre de referencia del deporte.</summary>
        public string? SportRefName { get; protected set; }

        /// <summary>Identificador numérico de referencia del deporte.</summary>
        public int? SportRefNumericalId { get; protected set; }

        /// <summary>Identificador de referencia de la liga.</summary>
        public string? LeagueRefId { get; protected set; }

        /// <summary>Etiqueta de referencia de la liga.</summary>
        public string? LeagueRefLabel { get; protected set; }

        /// <summary>Identificador numérico de referencia de la liga.</summary>
        public int? LeagueRefNumericalId { get; protected set; }

        /// <summary>Identificador de referencia del mercado.</summary>
        public string? MarketRefId { get; protected set; }

        /// <summary>Etiqueta de referencia del mercado.</summary>
        public string? MarketRefLabel { get; protected set; }

        /// <summary>Identificador numérico de referencia del mercado.</summary>
        public int? MarketRefNumericalId { get; protected set; }

        /// <summary>Identificador de referencia de la casa.</summary>
        public string? SportsbookRefId { get; protected set; }

        /// <summary>Etiqueta de referencia de la casa.</summary>
        public string? SportsbookRefName { get; protected set; }

        /// <summary>Identificador numérico de referencia de la casa.</summary>
        public int? SportsbookRefNumericalId { get; protected set; }

        /// <summary>Identificador de referencia de la competencia.</summary>
        public string? CompetitionRefId { get; protected set; }

        /// <summary>Etiqueta de referencia de la competencia.</summary>
        public string? CompetitionRefLabel { get; protected set; }

        /// <summary>Identificador numérico de referencia de la competencia.</summary>
        public int? CompetitionRefNumericalId { get; protected set; }

        /// <summary>Identificador externo del evento.</summary>
        public string? ExternalEventId { get; protected set; }

        /// <summary>Fecha y hora UTC exactas de inserción local.</summary>
        public DateTime CreatedAtUtc { get; protected set; }
    }
}
