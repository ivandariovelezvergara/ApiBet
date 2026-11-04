USE [Deportes];
GO

CREATE OR ALTER PROCEDURE dbo.spr_BestOdds_ValueBets
    @Bankroll                DECIMAL(18, 2) = 1000000, -- Banca actual en COP
    @MaxStakePercent         DECIMAL(18, 8) = 1.75,    -- Máximo por apuesta individual (1.75%)
    @MaxTotalExposurePercent DECIMAL(18, 8) = 18.0,    -- Tope de banca comprometida
    @KellyFraction           DECIMAL(18, 8) = 0.30,    -- Fracción de Kelly (0.30)
    @MaxPicks                INT            = 10,      -- Top N picks a retornar
    @MinExpectedValue        DECIMAL(18, 8) = 0.020,   -- EV mínimo (2.0%)
    @MaxExpectedValue        DECIMAL(18, 8) = 0.20,    -- EV máximo (20% para filtrar cuotas locas de 301.00)
    @MinOdds                 DECIMAL(18, 4) = 1.35,    -- Cuota mínima
    @MaxOdds                 DECIMAL(18, 4) = 3.80,    -- Cuota máxima rentable
    @MinMinutesToStart       INT            = -15,     -- Permite hasta 15 min de tolerancia
    @MaxMinutesToStart       INT            = 360,     -- Hasta 6 horas en Hora Colombia (cuando tiene hora)
    @StakeRoundToCop         DECIMAL(18, 0) = 1000,    -- Redondeo a múltiplos de $1,000 COP
    @MinStakeCop             DECIMAL(18, 0) = 5000,    -- Apuesta mínima en COP
    @IsMainLine              BIT            = 1,       -- 1 = Solo línea principal
    @SoloConHoraExacta       BIT            = 0        -- 0 = Usa los 20,200 registros | 1 = Solo los 562 de MarketOddsDelta
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Validaciones
    IF @Bankroll IS NULL OR @Bankroll <= 0
        THROW 50001, 'ValueBets: @Bankroll debe ser mayor a cero.', 1;

    IF @KellyFraction IS NULL OR @KellyFraction <= 0 OR @KellyFraction > 1
        THROW 50002, 'ValueBets: @KellyFraction debe estar entre 0.01 y 1.00.', 1;

    IF @MaxPicks IS NULL OR @MaxPicks <= 0 SET @MaxPicks = 10;
    IF @StakeRoundToCop IS NULL OR @StakeRoundToCop <= 0 SET @StakeRoundToCop = 1000;

    -- 2. Reloj Maestro en HORA COLOMBIA (Confirmado por Paso_7B de la radiografía)
    DECLARE @NowColombia     DATETIME2 = DATEADD(HOUR, -5, SYSUTCDATETIME());
    DECLARE @TodayColStr     CHAR(10)  = CONVERT(CHAR(10), CAST(@NowColombia AS DATE), 23);
    -- En la tarde/noche de Colombia, los partidos de las próximas horas ya tienen fecha UTC del día siguiente en su EventId
    DECLARE @TomorrowUtcStr  CHAR(10)  = CONVERT(CHAR(10), CAST(DATEADD(HOUR, 6, SYSUTCDATETIME()) AS DATE), 23);

    DECLARE @MaxStakeCop DECIMAL(18, 0)         = CAST(ROUND(@Bankroll * (@MaxStakePercent / 100.0), 0) AS DECIMAL(18, 0));
    DECLARE @MaxTotalExposureCop DECIMAL(18, 0) = CAST(ROUND(@Bankroll * (@MaxTotalExposurePercent / 100.0), 0) AS DECIMAL(18, 0));

    -- 3. Deportes configurados
    DECLARE @PrimeSports TABLE (SportName VARCHAR(64) NOT NULL PRIMARY KEY);
    INSERT INTO @PrimeSports (SportName) VALUES
        ('Basketball'), ('Tennis'), ('Baseball'),
        ('American Football'), ('MMA'), ('Hockey'), ('Ice Hockey');

    IF OBJECT_ID('tempdb..#Candidatos') IS NOT NULL
        DROP TABLE #Candidatos;

    -- 4. Procesamiento de los 20,200 registros con LEFT JOIN a MarketOddsDelta
    ;WITH EventosConHora AS
    (
        SELECT
            m.EventId,
            EventStartTime = MIN(m.EventStartTime),
            MinutosCol     = DATEDIFF(MINUTE, @NowColombia, MIN(m.EventStartTime))
        FROM dbo.MarketOddsDelta AS m
        GROUP BY m.EventId
    ),
    BestOddsSinDuplicados AS
    (
        -- Elimina las filas repetidas que vimos en la segunda tabla de tu radiografía
        SELECT
            b.*,
            DupRn = ROW_NUMBER() OVER (
                        PARTITION BY b.EventId, b.MarketType, b.Selection, ISNULL(b.Line, -999999), b.BestBook
                        ORDER BY b.BestDecimal DESC
                    )
        FROM dbo.BestOdds AS b
        WHERE (@IsMainLine IS NULL OR b.IsMainLine = @IsMainLine)
          AND b.BestDecimal BETWEEN @MinOdds AND @MaxOdds
          AND b.ConsensusProbability IS NOT NULL
          AND (b.PlayerName IS NULL OR b.PlayerName = '')
    ),
    Base AS
    (
        SELECT
            b.EventId,
            Partido           = b.EventName,
            Deporte           = LTRIM(RTRIM(s.Name)),
            Liga              = l.DisplayName,
            MarketId          = b.MarketType,
            Mercado           = m.Name,
            ApostarA          = b.Selection,
            Linea             = b.Line,
            Casa              = sb.DisplayName,
            Cuota             = b.BestDecimal,
            HoraInicioCol     = eh.EventStartTime,
            MinutosParaInicio = eh.MinutosCol,
            p                 = CASE
                                    WHEN b.ConsensusProbability > 1.0 THEN b.ConsensusProbability / 100.0
                                    ELSE b.ConsensusProbability
                                END,
            MarketNorm        = REPLACE(REPLACE(REPLACE(LOWER(m.Name), '_', ''), ' ', ''), '-', '')
        FROM BestOddsSinDuplicados AS b
        INNER JOIN dbo.Sports AS s ON s.Id = b.Sport
        INNER JOIN dbo.Leagues AS l ON l.Id = b.League
        INNER JOIN dbo.Markets AS m ON m.Id = b.MarketType
        INNER JOIN dbo.Sportsbooks AS sb ON sb.Id = b.BestBook
        INNER JOIN @PrimeSports AS ps ON ps.SportName = LTRIM(RTRIM(s.Name))
        LEFT JOIN EventosConHora AS eh ON eh.EventId = b.EventId
        WHERE b.DupRn = 1
          AND (
                -- CASO 1: El evento existe en MarketOddsDelta y está dentro de la ventana de minutos en Colombia
                (eh.EventId IS NOT NULL AND eh.MinutosCol BETWEEN @MinMinutesToStart AND @MaxMinutesToStart)
                OR
                -- CASO 2: El evento NO está en MarketOddsDelta (los otros 19,638 registros), filtramos por fecha actual/próxima en EventId
                (@SoloConHoraExacta = 0 AND eh.EventId IS NULL AND (
                    b.EventId LIKE '%' + @TodayColStr + '%'
                    OR b.EventId LIKE '%' + @TomorrowUtcStr + '%'
                ))
              )
    ),
    MercadosPrincipales AS
    (
        SELECT *
        FROM Base
        WHERE p > 0.08 AND p < 0.92
          AND (
                MarketNorm LIKE '%moneyline%'
                OR MarketNorm LIKE '%pointspread%'
                OR MarketNorm LIKE '%asianhandicap%'
                OR MarketNorm LIKE '%runline%'
                OR MarketNorm LIKE '%puckline%'
                OR MarketNorm LIKE '%gamespread%'
                OR MarketNorm LIKE '%setspread%'
              )
          AND MarketNorm NOT LIKE '%3way%'
          AND MarketNorm NOT LIKE '%regulation%'
          AND MarketNorm NOT LIKE '%total%'
          AND MarketNorm NOT LIKE '%btts%'
          AND MarketNorm NOT LIKE '%bothteam%'
          AND MarketNorm NOT LIKE '%championship%'
          AND MarketNorm NOT LIKE '%outright%'
          AND MarketNorm NOT LIKE '%winner%'
          AND MarketNorm NOT LIKE '%halftime%'
          AND MarketNorm NOT LIKE '%1st%'
          AND MarketNorm NOT LIKE '%period%'
          AND MarketNorm NOT LIKE '%quarter%'
          AND MarketNorm NOT LIKE '%half%'
          AND MarketNorm NOT LIKE '%inning%'
    ),
    CalculoEv AS
    (
        SELECT
            m.*,
            Implicita      = 1.0 / m.Cuota,
            EV             = (m.p * m.Cuota) - 1.0,
            b_neto         = m.Cuota - 1.0,
            FactorDeporte  = CASE
                                 WHEN m.Deporte IN ('Basketball', 'Baseball', 'American Football') THEN 1.00
                                 WHEN m.Deporte IN ('Hockey', 'Ice Hockey', 'Tennis') THEN 0.90
                                 WHEN m.Deporte = 'MMA' THEN 0.80
                                 ELSE 0.85
                             END
        FROM MercadosPrincipales AS m
    ),
    KellyOptimizado AS
    (
        SELECT
            *,
            KellyFull   = EV / b_neto,
            FactorFinal = @KellyFraction
                          * FactorDeporte
                          * (CASE WHEN Cuota > 2.40 THEN (2.40 / Cuota) ELSE 1.0 END)
        FROM CalculoEv
        WHERE EV >= @MinExpectedValue
          AND EV <= @MaxExpectedValue
    ),
    CalculoStakes AS
    (
        SELECT
            *,
            KellyApplied   = KellyFull * FactorFinal,
            ExpectedGrowth = (p * LOG(1.0 + ((KellyFull * FactorFinal) * b_neto)))
                           + ((1.0 - p) * LOG(1.0 - (KellyFull * FactorFinal))),
            StakeBruto     = CASE
                                WHEN (@Bankroll * (KellyFull * FactorFinal)) > @MaxStakeCop THEN @MaxStakeCop
                                ELSE (@Bankroll * (KellyFull * FactorFinal))
                             END
        FROM KellyOptimizado
        WHERE (KellyFull * FactorFinal) > 0
    )
    SELECT
        EventId, Partido, Deporte, Liga, Mercado, ApostarA, Linea, Casa, Cuota,
        HoraInicioCol, MinutosParaInicio, p, Implicita, EV, KellyFull, KellyApplied, ExpectedGrowth,
        Stake = CAST(ROUND(StakeBruto / @StakeRoundToCop, 0) * @StakeRoundToCop AS DECIMAL(18, 0))
    INTO #Candidatos
    FROM CalculoStakes;

    -- 5. Selección del Mejor Pick por Partido y Control de Exposición Total
    ;WITH MejorPorPartido AS
    (
        SELECT
            c.*,
            rn = ROW_NUMBER() OVER (
                PARTITION BY c.EventId
                ORDER BY c.ExpectedGrowth DESC, c.EV DESC, c.Cuota ASC
            )
        FROM #Candidatos AS c
        WHERE c.Stake >= @MinStakeCop
    ),
    CarteraActiva AS
    (
        SELECT
            m.*,
            PickOrder = ROW_NUMBER() OVER (ORDER BY m.ExpectedGrowth DESC, m.EV DESC),
            ExposicionAcumulada = SUM(m.Stake) OVER (
                ORDER BY m.ExpectedGrowth DESC, m.EV DESC
                ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
            )
        FROM MejorPorPartido AS m
        WHERE m.rn = 1
    )
    SELECT TOP (@MaxPicks)
        Pick                 = r.PickOrder,
        Senal                = CASE
                                 WHEN r.EV >= 0.05 AND r.Cuota <= 2.15 THEN '🔥 ELITE (+EV Alto)'
                                 WHEN r.EV >= 0.035 THEN '⭐ ALTA CONFIANZA'
                                 ELSE '✅ VALOR SOLIDO'
                               END,
        r.Deporte,
        r.Liga,
        r.Partido,
        MinutosFaltantes     = ISNULL(r.MinutosParaInicio, 0),
        EstadoHorario        = CASE
                                 WHEN r.MinutosParaInicio IS NOT NULL
                                      THEN CONCAT('En ', r.MinutosParaInicio, ' min (', FORMAT(r.HoraInicioCol, 'HH:mm'), ' COL)')
                                 ELSE 'Hoy / Próximas horas (Verificar hora en casa)'
                               END,
        r.Mercado,
        r.ApostarA,
        Linea                = r.Linea,
        Casa                 = r.Casa,
        CuotaActual          = CAST(r.Cuota AS DECIMAL(18, 3)),
        CuotaMinimaRentable  = CAST((1.0 + @MinExpectedValue) / r.p AS DECIMAL(18, 3)),
        ProbConsensoPct      = CAST(r.p * 100.0 AS DECIMAL(18, 2)),
        ProbImplicitaPct     = CAST(r.Implicita * 100.0 AS DECIMAL(18, 2)),
        EvPct                = CAST(r.EV * 100.0 AS DECIMAL(18, 2)),
        KellyAplicadoPct     = CAST(r.KellyApplied * 100.0 AS DECIMAL(18, 2)),
        StakeCop             = r.Stake,
        GananciaEsperadaCop  = CAST(ROUND(r.Stake * r.EV, 0) AS DECIMAL(18, 0)),
        RiesgoAcumuladoCop   = r.ExposicionAcumulada,
        Instruccion          = CONCAT(
                                 'Apostar $', FORMAT(r.Stake, 'N0', 'es-CO'), ' COP en ', r.Casa,
                                 ' a ', r.ApostarA,
                                 ' (', r.Mercado,
                                 CASE WHEN r.Linea IS NULL THEN '' ELSE CONCAT(' ', CAST(r.Linea AS DECIMAL(18, 2))) END,
                                 ') @ ', CAST(r.Cuota AS DECIMAL(18, 2)),
                                 ' [Min aceptable: ', CAST((1.0 + @MinExpectedValue) / r.p AS DECIMAL(18, 2)), ']',
                                 ' | EV: +', CAST(r.EV * 100.0 AS DECIMAL(18, 2)), '%'
                               )
    FROM CarteraActiva AS r
    WHERE r.ExposicionAcumulada <= @MaxTotalExposureCop
    ORDER BY r.PickOrder ASC;

    -- 6. Resumen Ejecutivo
    SELECT
        Deporte,
        Liga,
        PartidosConValor   = COUNT(DISTINCT EventId),
        PicksTotales       = COUNT(*),
        SurebetsDetectadas = 0,
        ConHoraExacta      = SUM(CASE WHEN MinutosParaInicio IS NOT NULL THEN 1 ELSE 0 END),
        EvMedioPct         = CAST(AVG(EV) * 100.0 AS DECIMAL(18, 2)),
        CuotaMedia         = CAST(AVG(Cuota) AS DECIMAL(18, 2))
    FROM #Candidatos
    WHERE Stake >= @MinStakeCop
    GROUP BY Deporte, Liga
    ORDER BY AVG(ExpectedGrowth) DESC, COUNT(DISTINCT EventId) DESC;

    DROP TABLE #Candidatos;
END;
GO
