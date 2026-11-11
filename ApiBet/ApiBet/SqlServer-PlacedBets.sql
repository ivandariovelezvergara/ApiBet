USE [Deportes];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

/* -------------------------------------------------------------------------- */
/* Tabla permanente de bitácora de apuestas ejecutadas y cierre de línea (CLV) */
/* -------------------------------------------------------------------------- */

IF OBJECT_ID(N'dbo.PlacedBets', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PlacedBets
    (
        Id                    INT            IDENTITY(1, 1) NOT NULL CONSTRAINT PK_PlacedBets PRIMARY KEY,
        BetKind               VARCHAR(16)    NOT NULL CONSTRAINT DF_PlacedBets_BetKind DEFAULT ('VALUE_BET'),
        Deporte               VARCHAR(64)    NOT NULL,
        Liga                  VARCHAR(256)   NOT NULL,
        Partido               VARCHAR(512)   NOT NULL,
        Mercado               VARCHAR(128)   NOT NULL,
        Seleccion             VARCHAR(256)   NOT NULL,
        Linea                 DECIMAL(18, 8) NULL,
        Casa                  VARCHAR(128)   NOT NULL,
        CuotaTomada           DECIMAL(18, 4) NOT NULL,
        CuotaMinimaRentable   DECIMAL(18, 4) NULL,
        ProbConsensoTomadaPct DECIMAL(18, 2) NULL,
        EvEstimadoPct         DECIMAL(18, 2) NOT NULL,
        StakeCop              DECIMAL(18, 0) NOT NULL,
        GananciaEsperadaCop   DECIMAL(18, 0) NOT NULL,
        UltimaCuotaMercado    DECIMAL(18, 4) NULL,
        UltimaProbConsensoPct DECIMAL(18, 2) NULL,
        ClvCuotaPct           AS (
                                  CASE
                                      WHEN UltimaCuotaMercado IS NOT NULL AND UltimaCuotaMercado > 1.0
                                          THEN CAST(((CuotaTomada / UltimaCuotaMercado) - 1.0) * 100.0 AS DECIMAL(18, 2))
                                      ELSE NULL
                                  END
                              ),
        Estado                VARCHAR(16)    NOT NULL CONSTRAINT DF_PlacedBets_Estado DEFAULT ('PENDING'),
        ProfitRealCop         DECIMAL(18, 0) NULL,
        PlacedAtCol           DATETIME2      NOT NULL CONSTRAINT DF_PlacedBets_PlacedAtCol DEFAULT (DATEADD(HOUR, -5, SYSUTCDATETIME())),
        ClosedAtCol           DATETIME2      NULL,
        LastClvUpdatedAtCol   DATETIME2      NULL
    );

    CREATE INDEX IX_PlacedBets_Estado ON dbo.PlacedBets (Estado);
    CREATE INDEX IX_PlacedBets_Partido ON dbo.PlacedBets (Partido);
END;
GO

/* -------------------------------------------------------------------------- */
/* Sincroniza la última cuota disponible en BestOdds para calcular el CLV     */
/* -------------------------------------------------------------------------- */

CREATE OR ALTER PROCEDURE dbo.spr_PlacedBets_SyncClosingOdds
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    IF NOT EXISTS (SELECT 1 FROM dbo.PlacedBets WITH (NOLOCK) WHERE Estado = 'PENDING')
        RETURN;

    DECLARE @NowColombia DATETIME2 = DATEADD(HOUR, -5, SYSUTCDATETIME());

    ;WITH CuotasActuales AS
    (
        SELECT
            Partido           = b.EventName,
            Mercado           = m.Name,
            Seleccion         = b.Selection,
            LineaNorm         = ISNULL(b.Line, -999999),
            Casa              = sb.DisplayName,
            Cuota             = COALESCE(
                                    (
                                        SELECT TOP 1 bb.[Decimal]
                                        FROM dbo.BestOddBooks AS bb WITH (NOLOCK)
                                        INNER JOIN dbo.Sportsbooks AS sbb WITH (NOLOCK) ON sbb.Id = bb.Book
                                        WHERE bb.BestOddId = b.Id
                                          AND bb.[Decimal] > 1.0
                                        ORDER BY bb.[Decimal] DESC
                                    ),
                                    b.BestDecimal
                                ),
            ConsensusDecimal  = b.ConsensusDecimal,
            ProbConsensoPct   = CAST(
                                    (CASE
                                         WHEN b.ConsensusProbability > 1.0 THEN b.ConsensusProbability / 100.0
                                         ELSE b.ConsensusProbability
                                     END) * 100.0
                                AS DECIMAL(18, 2)),
            rn                = ROW_NUMBER() OVER (
                                    PARTITION BY b.EventName, m.Name, b.Selection, ISNULL(b.Line, -999999)
                                    ORDER BY b.BestDecimal DESC
                                )
        FROM dbo.BestOdds AS b WITH (NOLOCK)
        INNER JOIN dbo.Markets AS m WITH (NOLOCK) ON m.Id = b.MarketType
        LEFT JOIN dbo.Sportsbooks AS sb WITH (NOLOCK) ON sb.Id = b.BestBook
        WHERE b.BestDecimal IS NOT NULL
          AND b.BestDecimal > 1.0
    )
    UPDATE pb
    SET
        pb.UltimaCuotaMercado    = ISNULL(ca.ConsensusDecimal, ca.Cuota),
        pb.UltimaProbConsensoPct = ISNULL(ca.ProbConsensoPct, pb.UltimaProbConsensoPct),
        pb.LastClvUpdatedAtCol   = @NowColombia
    FROM dbo.PlacedBets AS pb
    INNER JOIN CuotasActuales AS ca
        ON ca.rn = 1
       AND ca.Partido = pb.Partido
       AND ca.Mercado = pb.Mercado
       AND ca.Seleccion = pb.Seleccion
       AND ca.LineaNorm = ISNULL(pb.Linea, -999999)
    WHERE pb.Estado = 'PENDING'
      AND (
            ISNULL(pb.UltimaCuotaMercado, -1) <> ISNULL(ca.ConsensusDecimal, ca.Cuota)
            OR ISNULL(pb.UltimaProbConsensoPct, -1) <> ISNULL(ca.ProbConsensoPct, -1)
          );
END;
GO

/* -------------------------------------------------------------------------- */
/* Registra una apuesta ejecutada en dbo.PlacedBets                           */
/* -------------------------------------------------------------------------- */

CREATE OR ALTER PROCEDURE dbo.spr_PlacedBets_Insert
    @BetKind               VARCHAR(16)    = 'VALUE_BET',
    @Deporte               VARCHAR(64),
    @Liga                  VARCHAR(256),
    @Partido               VARCHAR(512),
    @Mercado               VARCHAR(128),
    @Seleccion             VARCHAR(256),
    @Linea                 DECIMAL(18, 8) = NULL,
    @Casa                  VARCHAR(128),
    @CuotaTomada           DECIMAL(18, 4),
    @CuotaMinimaRentable   DECIMAL(18, 4) = NULL,
    @ProbConsensoTomadaPct DECIMAL(18, 2) = NULL,
    @EvEstimadoPct         DECIMAL(18, 2),
    @StakeCop              DECIMAL(18, 0),
    @GananciaEsperadaCop   DECIMAL(18, 0) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @CuotaTomada IS NULL OR @CuotaTomada <= 1.0
        THROW 50010, 'PlacedBets: @CuotaTomada debe ser mayor a 1.00.', 1;

    IF @StakeCop IS NULL OR @StakeCop <= 0
        THROW 50011, 'PlacedBets: @StakeCop debe ser mayor a cero.', 1;

    DECLARE @NowColombia DATETIME2 = DATEADD(HOUR, -5, SYSUTCDATETIME());
    DECLARE @ExistingId INT;

    SELECT TOP 1 @ExistingId = pb.Id
    FROM dbo.PlacedBets AS pb
    WHERE pb.Estado = 'PENDING'
      AND pb.Partido = @Partido
      AND pb.Mercado = @Mercado
      AND pb.Seleccion = @Seleccion
      AND ISNULL(pb.Linea, -999999) = ISNULL(@Linea, -999999)
    ORDER BY pb.Id DESC;

    IF @ExistingId IS NULL
    BEGIN
        DECLARE @ExpectedCop DECIMAL(18, 0) = ISNULL(
            @GananciaEsperadaCop,
            CAST(ROUND(@StakeCop * (@EvEstimadoPct / 100.0), 0) AS DECIMAL(18, 0))
        );

        INSERT INTO dbo.PlacedBets
        (
            BetKind, Deporte, Liga, Partido, Mercado, Seleccion, Linea, Casa,
            CuotaTomada, CuotaMinimaRentable, ProbConsensoTomadaPct, EvEstimadoPct,
            StakeCop, GananciaEsperadaCop, UltimaCuotaMercado, UltimaProbConsensoPct,
            Estado, ProfitRealCop, PlacedAtCol, LastClvUpdatedAtCol
        )
        VALUES
        (
            ISNULL(NULLIF(LTRIM(RTRIM(@BetKind)), ''), 'VALUE_BET'),
            @Deporte,
            @Liga,
            @Partido,
            @Mercado,
            @Seleccion,
            @Linea,
            @Casa,
            @CuotaTomada,
            @CuotaMinimaRentable,
            @ProbConsensoTomadaPct,
            @EvEstimadoPct,
            @StakeCop,
            @ExpectedCop,
            @CuotaTomada,
            @ProbConsensoTomadaPct,
            'PENDING',
            NULL,
            @NowColombia,
            @NowColombia
        );

        SET @ExistingId = SCOPE_IDENTITY();
    END;

    EXEC dbo.spr_PlacedBets_SyncClosingOdds;

    SELECT
        pb.Id,
        pb.BetKind,
        pb.Deporte,
        pb.Liga,
        pb.Partido,
        pb.Mercado,
        pb.Seleccion,
        pb.Linea,
        pb.Casa,
        pb.CuotaTomada,
        pb.CuotaMinimaRentable,
        pb.ProbConsensoTomadaPct,
        pb.EvEstimadoPct,
        pb.StakeCop,
        pb.GananciaEsperadaCop,
        pb.UltimaCuotaMercado,
        pb.UltimaProbConsensoPct,
        pb.ClvCuotaPct,
        pb.Estado,
        pb.ProfitRealCop,
        pb.PlacedAtCol,
        pb.ClosedAtCol
    FROM dbo.PlacedBets AS pb
    WHERE pb.Id = @ExistingId;
END;
GO

/* -------------------------------------------------------------------------- */
/* Actualiza el resultado final (WON, LOST, PUSH, VOID, PENDING)              */
/* -------------------------------------------------------------------------- */

CREATE OR ALTER PROCEDURE dbo.spr_PlacedBets_UpdateStatus
    @Id     INT,
    @Estado VARCHAR(16)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Normalized VARCHAR(16) = UPPER(LTRIM(RTRIM(ISNULL(@Estado, ''))));
    IF @Normalized NOT IN ('PENDING', 'WON', 'LOST', 'PUSH', 'VOID')
        THROW 50012, 'PlacedBets: @Estado debe ser PENDING, WON, LOST, PUSH o VOID.', 1;

    DECLARE @NowColombia DATETIME2 = DATEADD(HOUR, -5, SYSUTCDATETIME());

    UPDATE pb
    SET
        pb.Estado        = @Normalized,
        pb.ProfitRealCop = CASE
                               WHEN @Normalized = 'WON'  THEN CAST(ROUND(pb.StakeCop * (pb.CuotaTomada - 1.0), 0) AS DECIMAL(18, 0))
                               WHEN @Normalized = 'LOST' THEN -pb.StakeCop
                               WHEN @Normalized IN ('PUSH', 'VOID') THEN 0
                               ELSE NULL
                           END,
        pb.ClosedAtCol   = CASE
                               WHEN @Normalized = 'PENDING' THEN NULL
                               ELSE @NowColombia
                           END
    FROM dbo.PlacedBets AS pb
    WHERE pb.Id = @Id;

    SELECT
        pb.Id,
        pb.BetKind,
        pb.Deporte,
        pb.Liga,
        pb.Partido,
        pb.Mercado,
        pb.Seleccion,
        pb.Linea,
        pb.Casa,
        pb.CuotaTomada,
        pb.CuotaMinimaRentable,
        pb.ProbConsensoTomadaPct,
        pb.EvEstimadoPct,
        pb.StakeCop,
        pb.GananciaEsperadaCop,
        pb.UltimaCuotaMercado,
        pb.UltimaProbConsensoPct,
        pb.ClvCuotaPct,
        pb.Estado,
        pb.ProfitRealCop,
        pb.PlacedAtCol,
        pb.ClosedAtCol
    FROM dbo.PlacedBets AS pb
    WHERE pb.Id = @Id;
END;
GO

/* -------------------------------------------------------------------------- */
/* Consulta la bitácora y las métricas globales de CLV / ROI                  */
/* -------------------------------------------------------------------------- */

CREATE OR ALTER PROCEDURE dbo.spr_PlacedBets_Get
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

    EXEC dbo.spr_PlacedBets_SyncClosingOdds;

    -- 1. Bitácora detallada
    SELECT
        pb.Id,
        pb.BetKind,
        pb.Deporte,
        pb.Liga,
        pb.Partido,
        pb.Mercado,
        pb.Seleccion,
        pb.Linea,
        pb.Casa,
        pb.CuotaTomada,
        pb.CuotaMinimaRentable,
        pb.ProbConsensoTomadaPct,
        pb.EvEstimadoPct,
        pb.StakeCop,
        pb.GananciaEsperadaCop,
        pb.UltimaCuotaMercado,
        pb.UltimaProbConsensoPct,
        pb.ClvCuotaPct,
        pb.Estado,
        pb.ProfitRealCop,
        pb.PlacedAtCol,
        pb.ClosedAtCol
    FROM dbo.PlacedBets AS pb
    ORDER BY
        CASE WHEN pb.Estado = 'PENDING' THEN 0 ELSE 1 END ASC,
        pb.PlacedAtCol DESC,
        pb.Id DESC;

    -- 2. Resumen de rendimiento y CLV
    SELECT
        TotalBets          = COUNT(*),
        PendingBets        = ISNULL(SUM(CASE WHEN Estado = 'PENDING' THEN 1 ELSE 0 END), 0),
        ResolvedBets       = ISNULL(SUM(CASE WHEN Estado IN ('WON', 'LOST', 'PUSH') THEN 1 ELSE 0 END), 0),
        WonBets            = ISNULL(SUM(CASE WHEN Estado = 'WON' THEN 1 ELSE 0 END), 0),
        LostBets           = ISNULL(SUM(CASE WHEN Estado = 'LOST' THEN 1 ELSE 0 END), 0),
        PushBets           = ISNULL(SUM(CASE WHEN Estado IN ('PUSH', 'VOID') THEN 1 ELSE 0 END), 0),
        TotalStakedCop     = ISNULL(SUM(StakeCop), 0),
        ResolvedStakedCop  = ISNULL(SUM(CASE WHEN Estado IN ('WON', 'LOST', 'PUSH') THEN StakeCop ELSE 0 END), 0),
        PendingExposureCop = ISNULL(SUM(CASE WHEN Estado = 'PENDING' THEN StakeCop ELSE 0 END), 0),
        RealizedProfitCop  = ISNULL(SUM(CASE WHEN Estado IN ('WON', 'LOST', 'PUSH') THEN ISNULL(ProfitRealCop, 0) ELSE 0 END), 0),
        ExpectedProfitCop  = ISNULL(SUM(GananciaEsperadaCop), 0),
        RoiPct             = CAST(
                                 CASE
                                     WHEN SUM(CASE WHEN Estado IN ('WON', 'LOST', 'PUSH') THEN StakeCop ELSE 0 END) > 0
                                         THEN (SUM(CASE WHEN Estado IN ('WON', 'LOST', 'PUSH') THEN ISNULL(ProfitRealCop, 0) ELSE 0 END) * 100.0)
                                              / SUM(CASE WHEN Estado IN ('WON', 'LOST', 'PUSH') THEN StakeCop ELSE 0 END)
                                     ELSE 0
                                 END
                             AS DECIMAL(18, 2)),
        AvgEvPct           = CAST(ISNULL(AVG(EvEstimadoPct), 0) AS DECIMAL(18, 2)),
        AvgClvPct          = CAST(ISNULL(AVG(ClvCuotaPct), 0) AS DECIMAL(18, 2)),
        PositiveClvRatePct = CAST(
                                 CASE
                                     WHEN COUNT(ClvCuotaPct) > 0
                                         THEN (SUM(CASE WHEN ClvCuotaPct > 0 THEN 1.0 ELSE 0.0 END) * 100.0) / COUNT(ClvCuotaPct)
                                     ELSE 0
                                 END
                             AS DECIMAL(18, 2))
    FROM dbo.PlacedBets;
END;
GO
