USE [Deportes];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[spr_BestOdds_ValueBets]
    @Bankroll                DECIMAL(18, 2) = 1000000, -- Banca actual en COP
    @MaxStakePercent         DECIMAL(18, 8) = 1.75,    -- Máximo por apuesta individual (1.75%)
    @MaxTotalExposurePercent DECIMAL(18, 8) = 18.0,    -- Tope de banca comprometida
    @KellyFraction           DECIMAL(18, 8) = 0.30,    -- Fracción de Kelly (0.30)
    @MaxPicks                INT            = 10,      -- Top N picks a retornar
    @MinExpectedValue        DECIMAL(18, 8) = 0.020,   -- EV mínimo (2.0%)
    @MaxExpectedValue        DECIMAL(18, 8) = 0.20,    -- EV máximo (20%)
    @MinOdds                 DECIMAL(18, 4) = 1.85,    -- Piso de cuota en 1.85
    @MaxOdds                 DECIMAL(18, 4) = 3.50,    -- Techo de cuota en 3.50
    @MinMinutesToStart       INT            = -15,     -- Permite hasta 15 min de tolerancia
    @MaxMinutesToStart       INT            = 360,     -- Hasta 6 horas en Hora Colombia
    @StakeRoundToCop         DECIMAL(18, 0) = 1000,    -- Redondeo a múltiplos de $1,000 COP
    @MinStakeCop             DECIMAL(18, 0) = 5000,    -- Apuesta mínima en COP
    @IsMainLine              BIT            = 1,       -- 1 = Solo línea principal
    @SoloConHoraExacta       BIT            = 0        -- 0 = Todos | 1 = Solo los de MarketOddsDelta
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

    -- 2. Reloj Maestro en HORA COLOMBIA (UTC-5) y ventana de fechas de respaldo
    DECLARE @NowColombia     DATETIME2 = DATEADD(HOUR, -5, SYSUTCDATETIME());
    DECLARE @TodayColDate    DATE      = CAST(@NowColombia AS DATE);
    DECLARE @Day0ColStr      CHAR(10)  = CONVERT(CHAR(10), @TodayColDate, 23);
    DECLARE @Day1ColStr      CHAR(10)  = CONVERT(CHAR(10), DATEADD(DAY, 1, @TodayColDate), 23);
    DECLARE @Day2ColStr      CHAR(10)  = CONVERT(CHAR(10), DATEADD(DAY, 2, @TodayColDate), 23);
    DECLARE @Day3ColStr      CHAR(10)  = CONVERT(CHAR(10), DATEADD(DAY, 3, @TodayColDate), 23);

    DECLARE @MaxStakeCop DECIMAL(18, 0)         = CAST(ROUND(@Bankroll * (@MaxStakePercent / 100.0), 0) AS DECIMAL(18, 0));
    DECLARE @MaxTotalExposureCop DECIMAL(18, 0) = CAST(ROUND(@Bankroll * (@MaxTotalExposurePercent / 100.0), 0) AS DECIMAL(18, 0));

    -- 3. Deportes configurados (Incluye Soccer / Football)
    DECLARE @PrimeSports TABLE (SportName VARCHAR(64) NOT NULL PRIMARY KEY);
    INSERT INTO @PrimeSports (SportName) VALUES
        ('Basketball'), ('Tennis'), ('Baseball'),
        ('American Football'), ('MMA'), ('Hockey'), ('Ice Hockey'),
        ('Soccer'), ('Football');

    IF OBJECT_ID('tempdb..#Candidatos') IS NOT NULL
        DROP TABLE #Candidatos;

    -- 4. Procesamiento con Unificación Canónica de 2 Vías y De-Vigging Dual (Consenso + Ancla Stake)
    ;WITH EventosConHora AS
    (
        SELECT
            m.EventId,
            EventStartTime = MIN(m.EventStartTime),
            MinutosCol     = DATEDIFF(MINUTE, @NowColombia, MIN(m.EventStartTime))
        FROM dbo.MarketOddsDelta AS m
        GROUP BY m.EventId
    ),
    RawOdds AS
    (
        SELECT
            b.*,
            BooksCount    = ISNULL(NULLIF((SELECT COUNT(*) FROM dbo.BestOddBooks AS bb WHERE bb.BestOddId = b.Id), 0), 1),
            StakeProb     = (
                                SELECT TOP 1 CAST(1.0 / NULLIF(bb.[Decimal], 0) AS DECIMAL(18, 8))
                                FROM dbo.BestOddBooks AS bb
                                WHERE bb.BestOddId = b.Id
                                  AND LOWER(bb.Book) LIKE '%stake%'
                                  AND bb.[Decimal] > 1.0
                                ORDER BY bb.[Decimal] ASC
                            ),
            AwayNorm      = REPLACE(REPLACE(REPLACE(LOWER(LTRIM(RTRIM(
                                CASE
                                    WHEN CHARINDEX(' @ ', b.EventName) > 0
                                        THEN LEFT(b.EventName, CHARINDEX(' @ ', b.EventName) - 1)
                                    ELSE b.EventName
                                END
                            ))), '-', ''), ' ', ''), '.', ''),
            HomeNorm      = REPLACE(REPLACE(REPLACE(LOWER(LTRIM(RTRIM(
                                CASE
                                    WHEN CHARINDEX(' @ ', b.EventName) > 0
                                        THEN SUBSTRING(b.EventName, CHARINDEX(' @ ', b.EventName) + 3, 500)
                                    ELSE ''
                                END
                            ))), '-', ''), ' ', ''), '.', ''),
            SelLower      = LOWER(LTRIM(RTRIM(b.Selection))),
            SelNorm       = REPLACE(REPLACE(REPLACE(LOWER(LTRIM(RTRIM(b.Selection))), '-', ''), ' ', ''), '.', ''),
            LineaMagnitud = ISNULL(ABS(b.Line), -999999),
            p_cons        = CASE
                                WHEN b.ConsensusProbability > 1.0 THEN CAST(b.ConsensusProbability / 100.0 AS DECIMAL(18, 8))
                                ELSE CAST(b.ConsensusProbability AS DECIMAL(18, 8))
                            END
        FROM dbo.BestOdds AS b
        WHERE (@IsMainLine IS NULL OR b.IsMainLine = @IsMainLine)
          AND b.ConsensusProbability IS NOT NULL
          AND b.ConsensusProbability > 0
          AND (b.PlayerName IS NULL OR b.PlayerName = '')
          -- Excluimos selecciones de empate explícitas ("Draw" / "Tie")
          AND LOWER(LTRIM(RTRIM(b.Selection))) NOT IN ('draw', 'tie', 'empate', 'x')
    ),
    ConLadoCanonico AS
    (
        SELECT
            r.*,
            -- Agrupa variantes de nombres de equipos entre casas (ej. "Rostock" vs "Rostock Seawolves") en los 2 lados reales del partido
            LadoCanonico = CASE
                               WHEN r.SelLower = 'over' OR r.SelLower LIKE 'over %' OR r.SelLower LIKE '% over' THEN 'OVER'
                               WHEN r.SelLower = 'under' OR r.SelLower LIKE 'under %' OR r.SelLower LIKE '% under' THEN 'UNDER'
                               WHEN r.Line IS NOT NULL AND r.Line < 0 THEN 'LINE_NEG'
                               WHEN r.Line IS NOT NULL AND r.Line > 0 THEN 'LINE_POS'
                               WHEN LEN(r.AwayNorm) >= 4 AND (
                                    CHARINDEX(r.SelNorm, r.AwayNorm) > 0
                                    OR CHARINDEX(r.AwayNorm, r.SelNorm) > 0
                                    OR CHARINDEX(LEFT(r.AwayNorm, 5), r.SelNorm) > 0
                               ) THEN 'AWAY'
                               WHEN LEN(r.HomeNorm) >= 4 AND (
                                    CHARINDEX(r.SelNorm, r.HomeNorm) > 0
                                    OR CHARINDEX(r.HomeNorm, r.SelNorm) > 0
                                    OR CHARINDEX(LEFT(r.HomeNorm, 5), r.SelNorm) > 0
                               ) THEN 'HOME'
                               ELSE r.SelNorm
                           END
        FROM RawOdds AS r
    ),
    BestOddsLimpio AS
    (
        SELECT
            c.*,
            -- Consenso ponderado por cantidad de casas cuando el mismo lado vino separado por nombre de equipo
            p_raw          = CAST(
                                 SUM(c.p_cons * c.BooksCount) OVER (PARTITION BY c.EventId, c.MarketType, c.LineaMagnitud, c.LadoCanonico)
                                 / NULLIF(SUM(c.BooksCount) OVER (PARTITION BY c.EventId, c.MarketType, c.LineaMagnitud, c.LadoCanonico), 0)
                             AS DECIMAL(18, 8)),
            p_stake_side   = MAX(ISNULL(c.StakeProb, -1.0)) OVER (PARTITION BY c.EventId, c.MarketType, c.LineaMagnitud, c.LadoCanonico),
            TotalBooksLado = SUM(c.BooksCount) OVER (PARTITION BY c.EventId, c.MarketType, c.LineaMagnitud, c.LadoCanonico),
            DupRn          = ROW_NUMBER() OVER (
                                 PARTITION BY c.EventId, c.MarketType, c.LineaMagnitud, c.LadoCanonico
                                 ORDER BY c.BestDecimal DESC, c.BooksCount DESC
                             )
        FROM ConLadoCanonico AS c
    ),
    AuditoriaMercado2Vias AS
    (
        SELECT
            b.*,
            LadosEncontrados = COUNT(*) OVER (PARTITION BY b.EventId, b.MarketType, b.LineaMagnitud),
            SumaProbConsenso = SUM(b.p_raw) OVER (PARTITION BY b.EventId, b.MarketType, b.LineaMagnitud),
            LadosStake       = SUM(CASE WHEN b.p_stake_side > 0 THEN 1 ELSE 0 END) OVER (PARTITION BY b.EventId, b.MarketType, b.LineaMagnitud),
            SumaProbStake    = SUM(CASE WHEN b.p_stake_side > 0 THEN b.p_stake_side ELSE 0 END) OVER (PARTITION BY b.EventId, b.MarketType, b.LineaMagnitud)
        FROM BestOddsLimpio AS b
        WHERE b.DupRn = 1
    ),
    DeViggingCalculado AS
    (
        SELECT
            a.*,
            -- 1. De-Vig multiplicativo sobre el consenso general de ambos lados
            p_cons_devig  = CASE
                                WHEN a.LadosEncontrados = 2 AND a.SumaProbConsenso BETWEEN 0.94 AND 1.15
                                    THEN a.p_raw / a.SumaProbConsenso
                                -- Si solo vino 1 lado, exigimos al menos 2 casas en ese lado y aplicamos haircut realista (4.5%)
                                WHEN a.LadosEncontrados = 1 AND a.TotalBooksLado >= 2
                                    THEN a.p_raw / 1.045
                                ELSE NULL
                            END,
            -- 2. De-Vig multiplicativo puro de Stake (cuando Stake cotiza los 2 lados y la mejor cuota está en Bwin/Betsson)
            p_stake_devig = CASE
                                WHEN a.LadosEncontrados = 2
                                     AND a.LadosStake = 2
                                     AND a.SumaProbStake BETWEEN 0.98 AND 1.15
                                     AND LOWER(a.BestBook) NOT LIKE '%stake%'
                                    THEN a.p_stake_side / a.SumaProbStake
                                ELSE NULL
                            END
        FROM AuditoriaMercado2Vias AS a
        WHERE a.BestDecimal BETWEEN @MinOdds AND @MaxOdds
          AND a.LadosEncontrados IN (1, 2)
    ),
    Base AS
    (
        SELECT
            b.EventId,
            Partido           = b.EventName,
            Deporte           = CASE WHEN LTRIM(RTRIM(s.Name)) = 'Football' THEN 'Soccer' ELSE LTRIM(RTRIM(s.Name)) END,
            Liga              = l.DisplayName,
            MarketId          = b.MarketType,
            Mercado           = m.Name,
            ApostarA          = b.Selection,
            Linea             = b.Line,
            Casa              = sb.DisplayName,
            Cuota             = b.BestDecimal,
            HoraInicioCol     = eh.EventStartTime,
            MinutosParaInicio = eh.MinutosCol,
            b.LadosEncontrados,
            b.SumaProbConsenso,
            p                 = CAST(
                                    (
                                        CASE
                                            -- Si la apuesta es en Bwin/Betsson y Stake tiene ambos lados más agudos que el promedio contaminado:
                                            WHEN b.p_stake_devig IS NOT NULL AND b.p_cons_devig IS NOT NULL AND b.p_stake_devig > b.p_cons_devig
                                                THEN (b.p_stake_devig * 0.65) + (b.p_cons_devig * 0.35)
                                            ELSE ISNULL(b.p_cons_devig, b.p_stake_devig)
                                        END
                                    ) *
                                    CASE
                                        WHEN b.BestDecimal > 2.60 THEN 0.982
                                        WHEN b.BestDecimal > 2.15 THEN 0.990
                                        ELSE 1.000
                                    END *
                                    CASE
                                        WHEN LOWER(b.BestBook) LIKE '%stake%' THEN 0.993
                                        ELSE 1.000
                                    END
                                AS DECIMAL(18, 8)),
            MarketNorm        = REPLACE(REPLACE(REPLACE(LOWER(m.Name), '_', ''), ' ', ''), '-', '')
        FROM DeViggingCalculado AS b
        INNER JOIN dbo.Sports AS s ON s.Id = b.Sport
        INNER JOIN dbo.Leagues AS l ON l.Id = b.League
        INNER JOIN dbo.Markets AS m ON m.Id = b.MarketType
        INNER JOIN dbo.Sportsbooks AS sb ON sb.Id = b.BestBook
        INNER JOIN @PrimeSports AS ps ON ps.SportName = LTRIM(RTRIM(s.Name))
        LEFT JOIN EventosConHora AS eh ON eh.EventId = b.EventId
        WHERE (b.p_cons_devig IS NOT NULL OR b.p_stake_devig IS NOT NULL)
          AND (
                (eh.EventId IS NOT NULL AND eh.MinutosCol BETWEEN @MinMinutesToStart AND @MaxMinutesToStart)
                OR
                (@SoloConHoraExacta = 0 AND eh.EventId IS NULL AND (
                    b.EventId LIKE '%' + @Day0ColStr + '%'
                    OR b.EventId LIKE '%' + @Day1ColStr + '%'
                    OR b.EventId LIKE '%' + @Day2ColStr + '%'
                    OR b.EventId LIKE '%' + @Day3ColStr + '%'
                ))
              )
    ),
    MercadosPrincipales AS
    (
        SELECT *
        FROM Base
        WHERE p > 0.08 AND p < 0.92
          AND (LadosEncontrados <> 2 OR SumaProbConsenso BETWEEN 0.94 AND 1.15)
          AND (
                -- REGLA 1: Deportes americanos, básquet, hockey, tenis y MMA (Moneyline, Spreads y Totales de partido completo)
                (
                    Deporte <> 'Soccer'
                    AND (
                        MarketNorm LIKE '%moneyline%'
                        OR MarketNorm LIKE '%pointspread%'
                        OR MarketNorm LIKE '%asianhandicap%'
                        OR MarketNorm LIKE '%runline%'
                        OR MarketNorm LIKE '%puckline%'
                        OR MarketNorm LIKE '%gamespread%'
                        OR MarketNorm LIKE '%setspread%'
                        OR (
                            (
                                MarketNorm LIKE '%totalpoints%'
                                OR MarketNorm LIKE '%totalgoals%'
                                OR MarketNorm LIKE '%totalruns%'
                                OR MarketNorm LIKE '%totalgames%'
                                OR MarketNorm = 'total'
                                OR MarketNorm LIKE '%overunder%'
                            )
                            AND Linea IS NOT NULL
                        )
                    )
                )
                OR
                -- REGLA 2: Exclusiva para SOCCER -> Solo mercados puros de 2 vías (Handicaps, Draw No Bet y Total Goals)
                (
                    Deporte = 'Soccer'
                    AND (
                        MarketNorm LIKE '%asianhandicap%'
                        OR MarketNorm LIKE '%goalspread%'
                        OR MarketNorm LIKE '%pointspread%'
                        OR MarketNorm LIKE '%handicap%'
                        OR MarketNorm LIKE '%drawnobet%'
                        OR MarketNorm LIKE '%totalgoals%'
                        OR MarketNorm LIKE '%total%'
                        OR MarketNorm LIKE '%overunder%'
                    )
                    AND MarketNorm NOT LIKE '%moneyline%'
                    AND MarketNorm NOT LIKE '%1x2%'
                    AND (MarketNorm LIKE '%drawnobet%' OR Linea IS NOT NULL)
                )
              )
          -- Exclusiones globales (Totales por equipo, par/impar, jugadores, mitades, cuartos, períodos)
          AND MarketNorm NOT LIKE '%teamtotal%'
          AND MarketNorm NOT LIKE '%hometotal%'
          AND MarketNorm NOT LIKE '%awaytotal%'
          AND MarketNorm NOT LIKE '%player%'
          AND MarketNorm NOT LIKE '%corner%'
          AND MarketNorm NOT LIKE '%card%'
          AND MarketNorm NOT LIKE '%oddeven%'
          AND MarketNorm NOT LIKE '%3way%'
          AND MarketNorm NOT LIKE '%regulation%'
          AND MarketNorm NOT LIKE '%btts%'
          AND MarketNorm NOT LIKE '%bothteam%'
          AND MarketNorm NOT LIKE '%championship%'
          AND MarketNorm NOT LIKE '%outright%'
          AND MarketNorm NOT LIKE '%winner%'
          AND MarketNorm NOT LIKE '%halftime%'
          AND MarketNorm NOT LIKE '%1st%'
          AND MarketNorm NOT LIKE '%2nd%'
          AND MarketNorm NOT LIKE '%3rd%'
          AND MarketNorm NOT LIKE '%4th%'
          AND MarketNorm NOT LIKE '%period%'
          AND MarketNorm NOT LIKE '%quarter%'
          AND MarketNorm NOT LIKE '%half%'
          AND MarketNorm NOT LIKE '%inning%'
    ),
    CalculoEv AS
    (
        SELECT
            m.*,
            Implicita     = CAST(1.0 / m.Cuota AS DECIMAL(18, 8)),
            EV            = CAST((m.p * m.Cuota) - 1.0 AS DECIMAL(18, 8)),
            b_neto        = CAST(m.Cuota - 1.0 AS DECIMAL(18, 8)),
            FactorDeporte = CASE
                                WHEN m.Deporte IN ('Basketball', 'Baseball', 'American Football') THEN 1.00
                                WHEN m.Deporte = 'Soccer' THEN 0.95
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
                          * (CASE WHEN LadosEncontrados = 2 THEN 1.00 ELSE 0.85 END)
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
        LadosEncontrados,
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
                                   WHEN r.EV >= 0.05 AND r.Cuota <= 2.15 AND r.LadosEncontrados = 2 THEN N'🔥 ELITE (De-Vig 2 Vías)'
                                   WHEN r.EV >= 0.035 THEN N'⭐ ALTA CONFIANZA'
                                   ELSE N'✅ VALOR SOLIDO'
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
                                   ' | EV Real: +', CAST(r.EV * 100.0 AS DECIMAL(18, 2)), '%'
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
        SurebetsDetectadas = SUM(CASE WHEN LadosEncontrados = 2 THEN 1 ELSE 0 END),
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
