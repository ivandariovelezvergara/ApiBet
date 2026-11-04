# Script SQL Server — ApiBet

Script para crear la base y todas las tablas que usa ApiBet (catálogos SharpAPI, mejores cuotas y metadatos de sincronización).

Ejecutar en SQL Server Management Studio o `sqlcmd` contra la instancia destino. La API usa la base `Deportes`.

La collation por defecto de SQL Server (`CI` = case-insensitive) cubre los joins de `Sport`, `League`, `MarketType` y casas de apuestas.

```sql
IF DB_ID(N'Deportes') IS NULL
BEGIN
    CREATE DATABASE [Deportes];
END
GO

USE [Deportes];
GO

/* -------------------------------------------------------------------------- */
/* Catálogo de deportes                                                       */
/* -------------------------------------------------------------------------- */

IF OBJECT_ID(N'dbo.Sports', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Sports
    (
        Id            VARCHAR(64)  NOT NULL CONSTRAINT PK_Sports PRIMARY KEY,
        Name          VARCHAR(128) NOT NULL,
        NumericalId   INT           NULL,
        EventCount    INT           NOT NULL,
        LiveCount     INT           NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.SportLeagues', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SportLeagues
    (
        SportId VARCHAR(64)  NOT NULL,
        Code    VARCHAR(128) NOT NULL,
        CONSTRAINT PK_SportLeagues PRIMARY KEY (SportId, Code),
        CONSTRAINT FK_SportLeagues_Sports_SportId
            FOREIGN KEY (SportId) REFERENCES dbo.Sports (Id) ON DELETE CASCADE
    );
END
GO

/* -------------------------------------------------------------------------- */
/* Catálogo de ligas                                                          */
/* -------------------------------------------------------------------------- */

IF OBJECT_ID(N'dbo.Leagues', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Leagues
    (
        Id          VARCHAR(256) NOT NULL CONSTRAINT PK_Leagues PRIMARY KEY,
        DisplayName VARCHAR(256) NOT NULL,
        NumericalId INT           NULL,
        SportId     VARCHAR(64)  NOT NULL,
        EventCount  INT           NOT NULL,
        LiveCount   INT           NOT NULL
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Leagues_SportId'
      AND object_id = OBJECT_ID(N'dbo.Leagues'))
BEGIN
    CREATE INDEX IX_Leagues_SportId ON dbo.Leagues (SportId);
END
GO

/* -------------------------------------------------------------------------- */
/* Catálogo de casas de apuestas                                              */
/* -------------------------------------------------------------------------- */

IF OBJECT_ID(N'dbo.Sportsbooks', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Sportsbooks
    (
        Id            VARCHAR(128) NOT NULL CONSTRAINT PK_Sportsbooks PRIMARY KEY,
        NumericalId   INT           NULL,
        DisplayName   VARCHAR(256) NOT NULL,
        ShortName     VARCHAR(128) NOT NULL,
        Category      VARCHAR(64)  NOT NULL,
        HasLiveOdds   BIT           NOT NULL,
        HasPlayerProps BIT          NOT NULL,
        HasLimits     BIT           NULL,
        IsSharp       BIT           NOT NULL,
        Status        VARCHAR(32)  NOT NULL,
        EventCount    INT           NOT NULL,
        LastUpdate    DATETIME2     NULL,
        RequiresTier  VARCHAR(32)  NOT NULL,
        ComingSoon    BIT           NOT NULL
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Sportsbooks_Status'
      AND object_id = OBJECT_ID(N'dbo.Sportsbooks'))
BEGIN
    CREATE INDEX IX_Sportsbooks_Status ON dbo.Sportsbooks (Status);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Sportsbooks_Category'
      AND object_id = OBJECT_ID(N'dbo.Sportsbooks'))
BEGIN
    CREATE INDEX IX_Sportsbooks_Category ON dbo.Sportsbooks (Category);
END
GO

IF OBJECT_ID(N'dbo.SportsbookRegions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SportsbookRegions
    (
        SportsbookId VARCHAR(128) NOT NULL,
        Code         VARCHAR(32)  NOT NULL,
        CONSTRAINT PK_SportsbookRegions PRIMARY KEY (SportsbookId, Code),
        CONSTRAINT FK_SportsbookRegions_Sportsbooks_SportsbookId
            FOREIGN KEY (SportsbookId) REFERENCES dbo.Sportsbooks (Id) ON DELETE CASCADE
    );
END
GO

/* -------------------------------------------------------------------------- */
/* Catálogo de mercados                                                       */
/* -------------------------------------------------------------------------- */

IF OBJECT_ID(N'dbo.Markets', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Markets
    (
        Id          VARCHAR(128) NOT NULL CONSTRAINT PK_Markets PRIMARY KEY,
        Name        VARCHAR(256) NOT NULL,
        NumericalId INT           NULL,
        EventCount  INT           NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.MarketSports', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MarketSports
    (
        MarketId VARCHAR(128) NOT NULL,
        SportId  VARCHAR(64)  NOT NULL,
        CONSTRAINT PK_MarketSports PRIMARY KEY (MarketId, SportId),
        CONSTRAINT FK_MarketSports_Markets_MarketId
            FOREIGN KEY (MarketId) REFERENCES dbo.Markets (Id) ON DELETE CASCADE
    );
END
GO

/* -------------------------------------------------------------------------- */
/* Mejores cuotas                                                             */
/* -------------------------------------------------------------------------- */

IF OBJECT_ID(N'dbo.BestOdds', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BestOdds
    (
        Id                    VARCHAR(64)  NOT NULL CONSTRAINT PK_BestOdds PRIMARY KEY,
        EventId               VARCHAR(256) NOT NULL,
        EventName             VARCHAR(512) NOT NULL,
        Sport                 VARCHAR(64)  NOT NULL,
        League                VARCHAR(256) NOT NULL,
        MarketType            VARCHAR(128) NOT NULL,
        Selection             VARCHAR(256) NOT NULL,
        Line                  DECIMAL(18, 8) NULL,
        IsMainLine            BIT           NOT NULL,
        BestAmerican          DECIMAL(18, 8) NULL,
        BestDecimal           DECIMAL(18, 8) NULL,
        BestProbability       DECIMAL(18, 8) NULL,
        ConsensusAmerican     DECIMAL(18, 8) NULL,
        ConsensusDecimal      DECIMAL(18, 8) NULL,
        ConsensusProbability  DECIMAL(18, 8) NULL,
        MarketHold            DECIMAL(18, 8) NULL,
        BestBook              VARCHAR(128) NULL,
        [Timestamp]           DATETIME2     NULL,
        PlayerName            VARCHAR(256) NULL,
        StatCategory          VARCHAR(128) NULL,
        CreatedAtUtc          DATETIME2     NOT NULL CONSTRAINT DF_BestOdds_CreatedAtUtc DEFAULT (SYSDATETIME())
    );
END
GO

IF COL_LENGTH(N'dbo.BestOdds', N'CreatedAtUtc') IS NULL
BEGIN
    ALTER TABLE dbo.BestOdds
        ADD CreatedAtUtc DATETIME2 NOT NULL
            CONSTRAINT DF_BestOdds_CreatedAtUtc DEFAULT (SYSDATETIME());
END
GO

IF OBJECT_ID(N'dbo.BestOdds', N'U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'dbo.DF_BestOdds_CreatedAtUtc', N'D') IS NOT NULL
        ALTER TABLE dbo.BestOdds DROP CONSTRAINT DF_BestOdds_CreatedAtUtc;

    ALTER TABLE dbo.BestOdds
        ADD CONSTRAINT DF_BestOdds_CreatedAtUtc DEFAULT (SYSDATETIME()) FOR CreatedAtUtc;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_BestOdds_Sport'
      AND object_id = OBJECT_ID(N'dbo.BestOdds'))
BEGIN
    CREATE INDEX IX_BestOdds_Sport ON dbo.BestOdds (Sport);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_BestOdds_EventId'
      AND object_id = OBJECT_ID(N'dbo.BestOdds'))
BEGIN
    CREATE INDEX IX_BestOdds_EventId ON dbo.BestOdds (EventId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_BestOdds_League'
      AND object_id = OBJECT_ID(N'dbo.BestOdds'))
BEGIN
    CREATE INDEX IX_BestOdds_League ON dbo.BestOdds (League);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_BestOdds_MarketType'
      AND object_id = OBJECT_ID(N'dbo.BestOdds'))
BEGIN
    CREATE INDEX IX_BestOdds_MarketType ON dbo.BestOdds (MarketType);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_BestOdds_BestBook'
      AND object_id = OBJECT_ID(N'dbo.BestOdds'))
BEGIN
    CREATE INDEX IX_BestOdds_BestBook ON dbo.BestOdds (BestBook);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_BestOdds_CreatedAtUtc'
      AND object_id = OBJECT_ID(N'dbo.BestOdds'))
BEGIN
    CREATE INDEX IX_BestOdds_CreatedAtUtc ON dbo.BestOdds (CreatedAtUtc);
END
GO

IF OBJECT_ID(N'dbo.BestOddBooks', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BestOddBooks
    (
        Id         VARCHAR(64)   NOT NULL CONSTRAINT PK_BestOddBooks PRIMARY KEY,
        BestOddId  VARCHAR(64)   NOT NULL,
        Book       VARCHAR(128)  NOT NULL,
        Sportsbook VARCHAR(128)  NOT NULL CONSTRAINT DF_BestOddBooks_Sportsbook DEFAULT (''),
        American   DECIMAL(18, 8) NULL,
        [Decimal]  DECIMAL(18, 8) NULL,
        Edge       DECIMAL(18, 8) NULL,
        CONSTRAINT FK_BestOddBooks_BestOdds_BestOddId
            FOREIGN KEY (BestOddId) REFERENCES dbo.BestOdds (Id) ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_BestOddBooks_BestOddId'
      AND object_id = OBJECT_ID(N'dbo.BestOddBooks'))
BEGIN
    CREATE INDEX IX_BestOddBooks_BestOddId ON dbo.BestOddBooks (BestOddId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_BestOddBooks_Sportsbook'
      AND object_id = OBJECT_ID(N'dbo.BestOddBooks'))
BEGIN
    CREATE INDEX IX_BestOddBooks_Sportsbook ON dbo.BestOddBooks (Sportsbook);
END
GO

/* -------------------------------------------------------------------------- */
/* Metadatos de sincronización                                                */
/* -------------------------------------------------------------------------- */

IF OBJECT_ID(N'dbo.CatalogSyncStates', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CatalogSyncStates
    (
        Id               INT          NOT NULL CONSTRAINT PK_CatalogSyncStates PRIMARY KEY,
        SourceUpdatedAt  DATETIME2    NULL,
        LastSyncedAtUtc  DATETIME2    NOT NULL
    );
END
GO
```

## Tablas incluidas

| Tabla | Uso |
| --- | --- |
| `Sports` | Catálogo `GET /sports` |
| `SportLeagues` | Ligas asociadas a cada deporte |
| `Leagues` | Catálogo `GET /leagues` |
| `Sportsbooks` | Catálogo `GET /sportsbooks` |
| `SportsbookRegions` | Regiones de cada casa |
| `Markets` | Catálogo `GET /markets` |
| `MarketSports` | Deportes asociados a cada mercado |
| `BestOdds` | Snapshot `GET /odds/best` |
| `BestOddBooks` | `all_books` de cada fila |
| `MarketOdds` | Snapshot `GET /odds` |
| `MarketOddsDelta` | Incremental `GET /odds/delta` |
| `ArbitrageOpportunities` | Snapshot `GET /opportunities/arbitrage` |
| `ArbitrageOpportunityLegs` | Patas (`legs`) de cada oportunidad |
| `CatalogSyncStates` | Última sincronización (1=Sports, 2=Leagues, 3=Sportsbooks, 4=Markets, 5=BestOdds, 6=MarketOdds, 7=Arbitrage, 8=MarketOddsDelta) |

## Connection string

```text
Server=lpc:.\SQLEXPRESS;Database=Deportes;User Id=UsuarioDeportes;Password=F1v3s0f7;TrustServerCertificate=True;Encrypt=True;
```

## Procedimiento de carga masiva — `dbo.spr_BestOdds_BulkLoad`

Lo usa `POST /api/odds/best/sync`. Recibe un JSON (array de cuotas con `books`) e inserta en `BestOdds` y `BestOddBooks` de una sola vez. `CreatedAtUtc` se llena con `SYSDATETIME()` en cada insert. Los filtros del sync salen solo de `OddsSync` en `appsettings.json`.

Ejecutar en la base `Deportes` después de crear las tablas.

```sql
USE [Deportes];
GO

CREATE OR ALTER PROCEDURE dbo.spr_BestOdds_BulkLoad
    @Json VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Json IS NULL OR LTRIM(RTRIM(@Json)) IN (N'', N'[]')
    BEGIN
        SELECT 0 AS OddCount, 0 AS BookCount;
        RETURN;
    END;

    BEGIN TRANSACTION;

    INSERT INTO dbo.BestOdds
    (
        Id, EventId, EventName, Sport, League, MarketType, Selection,
        Line, IsMainLine, BestAmerican, BestDecimal, BestProbability,
        ConsensusAmerican, ConsensusDecimal, ConsensusProbability,
        MarketHold, BestBook, [Timestamp], PlayerName, StatCategory, CreatedAtUtc
    )
    SELECT
        ISNULL(src.Id, REPLACE(CONVERT(VARCHAR(36), NEWID()), N'-', N'')),
        ISNULL(src.EventId, N''),
        ISNULL(src.EventName, N''),
        ISNULL(src.Sport, N''),
        ISNULL(src.League, N''),
        ISNULL(src.MarketType, N''),
        ISNULL(src.Selection, N''),
        src.Line,
        ISNULL(src.IsMainLine, 0),
        src.BestAmerican,
        src.BestDecimal,
        src.BestProbability,
        src.ConsensusAmerican,
        src.ConsensusDecimal,
        src.ConsensusProbability,
        src.MarketHold,
        src.BestBook,
        src.[Timestamp],
        src.PlayerName,
        src.StatCategory,
        SYSDATETIME()
    FROM OPENJSON(@Json)
    WITH
    (
        Id                   VARCHAR(64)    '$.id',
        EventId              VARCHAR(256)   '$.eventId',
        EventName            VARCHAR(512)   '$.eventName',
        Sport                VARCHAR(64)    '$.sport',
        League               VARCHAR(256)   '$.league',
        MarketType           VARCHAR(128)   '$.marketType',
        Selection            VARCHAR(256)   '$.selection',
        Line                 DECIMAL(18, 8)  '$.line',
        IsMainLine           BIT             '$.isMainLine',
        BestAmerican         DECIMAL(18, 8)  '$.bestAmerican',
        BestDecimal          DECIMAL(18, 8)  '$.bestDecimal',
        BestProbability      DECIMAL(18, 8)  '$.bestProbability',
        ConsensusAmerican    DECIMAL(18, 8)  '$.consensusAmerican',
        ConsensusDecimal     DECIMAL(18, 8)  '$.consensusDecimal',
        ConsensusProbability DECIMAL(18, 8)  '$.consensusProbability',
        MarketHold           DECIMAL(18, 8)  '$.marketHold',
        BestBook             VARCHAR(128)   '$.bestBook',
        [Timestamp]          DATETIME2       '$.timestamp',
        PlayerName           VARCHAR(256)   '$.playerName',
        StatCategory         VARCHAR(128)   '$.statCategory'
    ) AS src;

    DECLARE @OddCount INT = @@ROWCOUNT;

    INSERT INTO dbo.BestOddBooks
    (
        Id, BestOddId, Book, Sportsbook, American, [Decimal], Edge
    )
    SELECT
        ISNULL(book.Id, REPLACE(CONVERT(VARCHAR(36), NEWID()), N'-', N'')),
        parent.Id,
        ISNULL(book.Book, N''),
        ISNULL(NULLIF(book.Sportsbook, N''), ISNULL(book.Book, N'')),
        book.American,
        book.[Decimal],
        book.Edge
    FROM OPENJSON(@Json)
    WITH
    (
        Id    VARCHAR(64)  '$.id',
        -- OPENJSON exige nvarchar(max) solo con AS JSON; no se persiste así en la tabla.
        Books NVARCHAR(MAX) '$.books' AS JSON
    ) AS parent
    CROSS APPLY OPENJSON(parent.Books)
    WITH
    (
        Id         VARCHAR(64)    '$.id',
        Book       VARCHAR(128)   '$.book',
        Sportsbook VARCHAR(128)   '$.sportsbook',
        American   DECIMAL(18, 8)  '$.american',
        [Decimal]  DECIMAL(18, 8)  '$.decimal',
        Edge       DECIMAL(18, 8)  '$.edge'
    ) AS book
    WHERE parent.Id IS NOT NULL;

    DECLARE @BookCount INT = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT @OddCount AS OddCount, @BookCount AS BookCount;
END;
GO
```

### JSON de ejemplo

```json
[
  {
    "id": "abc123",
    "eventId": "evt-1",
    "eventName": "Team A vs Team B",
    "sport": "basketball",
    "league": "nba",
    "marketType": "moneyline",
    "selection": "Team A",
    "line": null,
    "isMainLine": true,
    "bestAmerican": -110,
    "bestDecimal": 1.91,
    "bestProbability": 0.524,
    "consensusAmerican": -108,
    "consensusDecimal": 1.90,
    "consensusProbability": 0.519,
    "marketHold": 4.5,
    "bestBook": "pinnacle",
    "timestamp": "2026-10-20T13:00:00Z",
    "playerName": null,
    "statCategory": null,
    "books": [
      {
        "id": "book1",
        "book": "pinnacle",
        "sportsbook": "pinnacle",
        "american": -110,
        "decimal": 1.91,
        "edge": 1.2
      }
    ]
  }
]
```

## Exportar mejores cuotas a CSV (`spr_BestOdds_CsvData` + `spr_BestOdds_ExportCsv`)

Lo dispara `GET /api/odds/best`. Exporta con **bcp** vía `xp_cmdshell` los eventos de **mañana** (`EventId` contiene `yyyy-MM-dd` del día siguiente).

Archivo por defecto: `C:\ApiBet\Exports\BestOdds_yyyy-MM-dd.csv`.

La cuenta de servicio de SQL Server (`NT Service\MSSQL$SQLEXPRESS`) debe poder escribir en esa carpeta. `bcp` debe estar en el PATH del servicio.

### Una sola vez: habilitar `xp_cmdshell`

```sql
USE [Deportes];
GO

EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'xp_cmdshell', 1;
RECONFIGURE;
GO
```

### Datos del CSV y exportación

```sql
USE [Deportes];
GO

IF OBJECT_ID(N'dbo.fn_ToCsv', N'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_ToCsv;
GO

CREATE OR ALTER PROCEDURE dbo.spr_BestOdds_CsvData
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EventDate VARCHAR(10) = CONVERT(VARCHAR(10), DATEADD(day, 1, GETDATE()), 23);

    SELECT
        EventId,
        EventName,
        SportName,
        LeagueName,
        MarketName,
        Selection,
        Line,
        IsMainLine,
        BestAmerican,
        BestDecimal,
        BestProbability,
        ConsensusAmerican,
        ConsensusDecimal,
        ConsensusProbability,
        MarketHold,
        BestBookDisplayName,
        Book,
        SportsbookDisplayName,
        BookAmerican,
        BookDecimal,
        Edge
    FROM
    (
        SELECT
            0 AS SortKey,
            CAST('EventId' AS VARCHAR(512)) AS EventId,
            CAST('EventName' AS VARCHAR(512)) AS EventName,
            CAST('SportName' AS VARCHAR(128)) AS SportName,
            CAST('LeagueName' AS VARCHAR(256)) AS LeagueName,
            CAST('MarketName' AS VARCHAR(256)) AS MarketName,
            CAST('Selection' AS VARCHAR(256)) AS Selection,
            CAST('Line' AS VARCHAR(64)) AS Line,
            CAST('IsMainLine' AS VARCHAR(8)) AS IsMainLine,
            CAST('BestAmerican' AS VARCHAR(64)) AS BestAmerican,
            CAST('BestDecimal' AS VARCHAR(64)) AS BestDecimal,
            CAST('BestProbability' AS VARCHAR(64)) AS BestProbability,
            CAST('ConsensusAmerican' AS VARCHAR(64)) AS ConsensusAmerican,
            CAST('ConsensusDecimal' AS VARCHAR(64)) AS ConsensusDecimal,
            CAST('ConsensusProbability' AS VARCHAR(64)) AS ConsensusProbability,
            CAST('MarketHold' AS VARCHAR(64)) AS MarketHold,
            CAST('BestBookDisplayName' AS VARCHAR(256)) AS BestBookDisplayName,
            CAST('Book' AS VARCHAR(128)) AS Book,
            CAST('SportsbookDisplayName' AS VARCHAR(256)) AS SportsbookDisplayName,
            CAST('BookAmerican' AS VARCHAR(64)) AS BookAmerican,
            CAST('BookDecimal' AS VARCHAR(64)) AS BookDecimal,
            CAST('Edge' AS VARCHAR(64)) AS Edge
        UNION ALL
        SELECT
            1,
            ISNULL(b.EventId, ''),
            ISNULL(b.EventName, ''),
            ISNULL(s.Name, ''),
            ISNULL(l.DisplayName, ''),
            ISNULL(m.Name, ''),
            ISNULL(b.Selection, ''),
            ISNULL(CONVERT(VARCHAR(64), b.Line), ''),
            ISNULL(CONVERT(VARCHAR(1), CONVERT(INT, b.IsMainLine)), ''),
            ISNULL(CONVERT(VARCHAR(64), b.BestAmerican), ''),
            ISNULL(CONVERT(VARCHAR(64), b.BestDecimal), ''),
            ISNULL(CONVERT(VARCHAR(64), b.BestProbability), ''),
            ISNULL(CONVERT(VARCHAR(64), b.ConsensusAmerican), ''),
            ISNULL(CONVERT(VARCHAR(64), b.ConsensusDecimal), ''),
            ISNULL(CONVERT(VARCHAR(64), b.ConsensusProbability), ''),
            ISNULL(CONVERT(VARCHAR(64), b.MarketHold), ''),
            ISNULL(sb.DisplayName, ''),
            ISNULL(bo.Book, ''),
            ISNULL(sbb.DisplayName, ''),
            ISNULL(CONVERT(VARCHAR(64), bo.American), ''),
            ISNULL(CONVERT(VARCHAR(64), bo.[Decimal]), ''),
            ISNULL(CONVERT(VARCHAR(64), bo.Edge), '')
        FROM dbo.BestOdds AS b
        INNER JOIN dbo.BestOddBooks AS bo
            ON bo.BestOddId = b.Id
        INNER JOIN dbo.Sports AS s
            ON s.Id = b.Sport
        INNER JOIN dbo.Leagues AS l
            ON l.Id = b.League
        INNER JOIN dbo.Markets AS m
            ON m.Id = b.MarketType
        INNER JOIN dbo.Sportsbooks AS sb
            ON sb.Id = b.BestBook
        INNER JOIN dbo.Sportsbooks AS sbb
            ON sbb.Id = bo.Sportsbook
        WHERE b.EventId LIKE '%' + @EventDate + '%'
    ) AS rows
    ORDER BY SortKey, EventId, Book;
END;
GO

CREATE OR ALTER PROCEDURE dbo.spr_BestOdds_ExportCsv
    @OutputFolder VARCHAR(512) = 'C:\ApiBet\Exports',
    @Server       VARCHAR(128) = '.\SQLEXPRESS',
    @SqlUser      VARCHAR(128) = NULL,
    @SqlPassword  VARCHAR(128) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF CONVERT(INT, (SELECT value_in_use FROM sys.configurations WHERE name = 'xp_cmdshell')) <> 1
    BEGIN
        RAISERROR('xp_cmdshell is disabled. Enable it once in SSMS (SqlServer-CreateTables.md).', 16, 1);
        RETURN;
    END;

    IF @OutputFolder IS NULL OR LTRIM(RTRIM(@OutputFolder)) = ''
    BEGIN
        RAISERROR('OutputFolder is required.', 16, 1);
        RETURN;
    END;

    DECLARE @EventDate VARCHAR(10) = CONVERT(VARCHAR(10), DATEADD(day, 1, GETDATE()), 23);
    DECLARE @Folder VARCHAR(512) = LTRIM(RTRIM(@OutputFolder));

    IF RIGHT(@Folder, 1) = '\'
        SET @Folder = LEFT(@Folder, LEN(@Folder) - 1);

    DECLARE @OutputFile VARCHAR(768) = @Folder + '\BestOdds_' + @EventDate + '.csv';
    DECLARE @Mkdir VARCHAR(8000) = 'if not exist "' + @Folder + '" mkdir "' + @Folder + '"';
    DECLARE @Auth VARCHAR(512);
    DECLARE @Bcp VARCHAR(8000);

    IF @SqlUser IS NULL OR LTRIM(RTRIM(@SqlUser)) = ''
        SET @Auth = ' -T';
    ELSE
        SET @Auth = ' -U "' + @SqlUser + '" -P "' + ISNULL(@SqlPassword, '') + '"';

    SET @Bcp = 'bcp "EXEC Deportes.dbo.spr_BestOdds_CsvData" queryout "'
             + @OutputFile
             + '" -c -C 1252 -t"," -r"\n" -S "'
             + @Server
             + '" -d Deportes'
             + @Auth;

    EXEC xp_cmdshell @Mkdir, no_output;

    DECLARE @BcpOut TABLE (Line VARCHAR(8000) NULL);

    INSERT INTO @BcpOut (Line)
    EXEC xp_cmdshell @Bcp;

    DECLARE @BcpText VARCHAR(MAX);

    SELECT @BcpText = STRING_AGG(CAST(Line AS VARCHAR(MAX)), CHAR(10))
    FROM @BcpOut
    WHERE Line IS NOT NULL;

    IF @BcpText IS NULL
       OR @BcpText LIKE '%not recognized%'
       OR @BcpText LIKE '%Error%'
       OR @BcpText LIKE '%error%'
       OR @BcpText NOT LIKE '%rows copied%'
    BEGIN
        DECLARE @BcpError VARCHAR(2047) = LEFT(ISNULL(@BcpText, 'bcp returned no output.'), 2047);
        RAISERROR('bcp export failed: %s', 16, 1, @BcpError);
        RETURN;
    END;

    SELECT
        FilePath  = @OutputFile,
        BcpOutput = @BcpText;
END;
GO
```

```sql
EXEC dbo.spr_BestOdds_ExportCsv
    @OutputFolder = 'C:\ApiBet\Exports',
    @Server = '.\SQLEXPRESS',
    @SqlUser = 'UsuarioDeportes',
    @SqlPassword = 'F1v3s0f7';
```

## Value bets desde BestOdds — `spr_BestOdds_ValueBets`

Lo usa `GET /api/odds/best` con **solo** `ValueModel` de `appsettings.json`. Cruza `BestOdds` con `MarketOddsDelta` (ventana 45–240 min, hora local). El procedimiento vigente está en `SqlServer-spr_BestOdds_ValueBets.sql`.


## Cuotas de mercado (GET /odds)

Tabla `MarketOdds` (distinta de `BestOdds`) y procedimiento `dbo.spr_MarketOdds_BulkLoad`. Lo dispara `POST /api/odds/sync` con **solo** `OddsFeed` de `appsettings.json`. `CreatedAtUtc` se llena al insertar. Ejecutar contra la base `Deportes`. Si la tabla ya existe, ejecuta también el `ALTER` de `CreatedAtUtc`.

```sql
/* -------------------------------------------------------------------------- */
/* Cuotas de mercado (GET /odds) — tabla distinta de BestOdds                 */
/* -------------------------------------------------------------------------- */

IF OBJECT_ID(N'dbo.MarketOdds', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MarketOdds
    (
        Id                       VARCHAR(64)    NOT NULL CONSTRAINT PK_MarketOdds PRIMARY KEY,
        Sportsbook               VARCHAR(128)   NOT NULL,
        EventId                  VARCHAR(256)   NOT NULL,
        EventUuid                VARCHAR(64)    NULL,
        Sport                    VARCHAR(64)    NOT NULL,
        League                   VARCHAR(256)   NOT NULL,
        HomeTeam                 VARCHAR(256)   NULL,
        AwayTeam                 VARCHAR(256)   NULL,
        MarketType               VARCHAR(128)   NOT NULL,
        Selection                VARCHAR(256)   NOT NULL,
        SelectionType            VARCHAR(64)    NULL,
        TeamSide                 VARCHAR(64)    NULL,
        OddsAmerican             DECIMAL(18, 8)  NULL,
        OddsDecimal              DECIMAL(18, 8)  NULL,
        OddsProbability          DECIMAL(18, 8)  NULL,
        Line                     DECIMAL(18, 8)  NULL,
        EventStartTime           DATETIME2       NULL,
        IsLive                   BIT             NOT NULL CONSTRAINT DF_MarketOdds_IsLive DEFAULT (0),
        IsActive                 BIT             NOT NULL CONSTRAINT DF_MarketOdds_IsActive DEFAULT (1),
        [Timestamp]              DATETIME2       NULL,
        DeepLink                 VARCHAR(512)   NULL,
        MarketId                 VARCHAR(128)   NULL,
        SelectionId              VARCHAR(128)   NULL,
        ExternalMarketId         VARCHAR(128)   NULL,
        IsAlternateLine          BIT             NOT NULL CONSTRAINT DF_MarketOdds_IsAlternateLine DEFAULT (0),
        IsMainLine               BIT             NOT NULL CONSTRAINT DF_MarketOdds_IsMainLine DEFAULT (0),
        IsStalePregamePrice      BIT             NOT NULL CONSTRAINT DF_MarketOdds_IsStalePregamePrice DEFAULT (0),
        IsPlayerProp             BIT             NOT NULL CONSTRAINT DF_MarketOdds_IsPlayerProp DEFAULT (0),
        IsImpossibleScoreline    BIT             NOT NULL CONSTRAINT DF_MarketOdds_IsImpossibleScoreline DEFAULT (0),
        SeasonType               VARCHAR(64)    NULL,
        HomeId                   VARCHAR(128)   NULL,
        HomeNumericalId          INT             NULL,
        HomeName                 VARCHAR(256)   NULL,
        HomeLogo                 VARCHAR(512)   NULL,
        HomeAbbreviation         VARCHAR(32)    NULL,
        HomeCity                 VARCHAR(128)   NULL,
        HomeMascot               VARCHAR(128)   NULL,
        HomeConference           VARCHAR(64)    NULL,
        HomeDivision             VARCHAR(128)   NULL,
        AwayId                   VARCHAR(128)   NULL,
        AwayNumericalId          INT             NULL,
        AwayName                 VARCHAR(256)   NULL,
        AwayLogo                 VARCHAR(512)   NULL,
        AwayAbbreviation         VARCHAR(32)    NULL,
        AwayCity                 VARCHAR(128)   NULL,
        AwayMascot               VARCHAR(128)   NULL,
        AwayConference           VARCHAR(64)    NULL,
        AwayDivision             VARCHAR(128)   NULL,
        SportRefId               VARCHAR(64)    NULL,
        SportRefName             VARCHAR(128)   NULL,
        SportRefNumericalId      INT             NULL,
        LeagueRefId              VARCHAR(256)   NULL,
        LeagueRefLabel           VARCHAR(256)   NULL,
        LeagueRefNumericalId     INT             NULL,
        MarketRefId              VARCHAR(128)   NULL,
        MarketRefLabel           VARCHAR(128)   NULL,
        MarketRefNumericalId     INT             NULL,
        SportsbookRefId          VARCHAR(128)   NULL,
        SportsbookRefName        VARCHAR(128)   NULL,
        SportsbookRefNumericalId INT             NULL,
        CompetitionRefId         VARCHAR(256)   NULL,
        CompetitionRefLabel      VARCHAR(256)   NULL,
        CompetitionRefNumericalId INT            NULL,
        ExternalEventId          VARCHAR(64)    NULL,
        CreatedAtUtc             DATETIME2       NOT NULL CONSTRAINT DF_MarketOdds_CreatedAtUtc DEFAULT (SYSDATETIME())
    );
END
GO

IF COL_LENGTH(N'dbo.MarketOdds', N'CreatedAtUtc') IS NULL
BEGIN
    ALTER TABLE dbo.MarketOdds
        ADD CreatedAtUtc DATETIME2 NOT NULL
            CONSTRAINT DF_MarketOdds_CreatedAtUtc DEFAULT (SYSDATETIME());
END
GO

IF OBJECT_ID(N'dbo.MarketOdds', N'U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'dbo.DF_MarketOdds_CreatedAtUtc', N'D') IS NOT NULL
        ALTER TABLE dbo.MarketOdds DROP CONSTRAINT DF_MarketOdds_CreatedAtUtc;

    ALTER TABLE dbo.MarketOdds
        ADD CONSTRAINT DF_MarketOdds_CreatedAtUtc DEFAULT (SYSDATETIME()) FOR CreatedAtUtc;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOdds_Sport' AND object_id = OBJECT_ID(N'dbo.MarketOdds'))
BEGIN
    CREATE INDEX IX_MarketOdds_Sport ON dbo.MarketOdds (Sport);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOdds_EventId' AND object_id = OBJECT_ID(N'dbo.MarketOdds'))
BEGIN
    CREATE INDEX IX_MarketOdds_EventId ON dbo.MarketOdds (EventId);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOdds_League' AND object_id = OBJECT_ID(N'dbo.MarketOdds'))
BEGIN
    CREATE INDEX IX_MarketOdds_League ON dbo.MarketOdds (League);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOdds_MarketType' AND object_id = OBJECT_ID(N'dbo.MarketOdds'))
BEGIN
    CREATE INDEX IX_MarketOdds_MarketType ON dbo.MarketOdds (MarketType);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOdds_Sportsbook' AND object_id = OBJECT_ID(N'dbo.MarketOdds'))
BEGIN
    CREATE INDEX IX_MarketOdds_Sportsbook ON dbo.MarketOdds (Sportsbook);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOdds_EventStartTime' AND object_id = OBJECT_ID(N'dbo.MarketOdds'))
BEGIN
    CREATE INDEX IX_MarketOdds_EventStartTime ON dbo.MarketOdds (EventStartTime);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOdds_CreatedAtUtc' AND object_id = OBJECT_ID(N'dbo.MarketOdds'))
BEGIN
    CREATE INDEX IX_MarketOdds_CreatedAtUtc ON dbo.MarketOdds (CreatedAtUtc);
END
GO

CREATE OR ALTER PROCEDURE dbo.spr_MarketOdds_BulkLoad
    @Json VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Json IS NULL OR LTRIM(RTRIM(@Json)) = N''
    BEGIN
        SELECT 0 AS OddCount;
        RETURN;
    END;

    BEGIN TRANSACTION;

    INSERT INTO dbo.MarketOdds
    (
        Id, Sportsbook, EventId, EventUuid, Sport, League, HomeTeam, AwayTeam,
        MarketType, Selection, SelectionType, TeamSide,
        OddsAmerican, OddsDecimal, OddsProbability, Line, EventStartTime,
        IsLive, IsActive, [Timestamp], DeepLink, MarketId, SelectionId, ExternalMarketId,
        IsAlternateLine, IsMainLine, IsStalePregamePrice, IsPlayerProp, IsImpossibleScoreline,
        SeasonType,
        HomeId, HomeNumericalId, HomeName, HomeLogo, HomeAbbreviation, HomeCity, HomeMascot, HomeConference, HomeDivision,
        AwayId, AwayNumericalId, AwayName, AwayLogo, AwayAbbreviation, AwayCity, AwayMascot, AwayConference, AwayDivision,
        SportRefId, SportRefName, SportRefNumericalId,
        LeagueRefId, LeagueRefLabel, LeagueRefNumericalId,
        MarketRefId, MarketRefLabel, MarketRefNumericalId,
        SportsbookRefId, SportsbookRefName, SportsbookRefNumericalId,
        CompetitionRefId, CompetitionRefLabel, CompetitionRefNumericalId,
        ExternalEventId, CreatedAtUtc
    )
    SELECT
        ISNULL(NULLIF(src.Id, N''), REPLACE(CONVERT(VARCHAR(36), NEWID()), N'-', N'')),
        ISNULL(src.Sportsbook, N''),
        ISNULL(src.EventId, N''),
        src.EventUuid,
        ISNULL(src.Sport, N''),
        ISNULL(src.League, N''),
        src.HomeTeam,
        src.AwayTeam,
        ISNULL(src.MarketType, N''),
        ISNULL(src.Selection, N''),
        src.SelectionType,
        src.TeamSide,
        src.OddsAmerican,
        src.OddsDecimal,
        src.OddsProbability,
        src.Line,
        src.EventStartTime,
        ISNULL(src.IsLive, 0),
        ISNULL(src.IsActive, 1),
        src.[Timestamp],
        src.DeepLink,
        src.MarketId,
        src.SelectionId,
        src.ExternalMarketId,
        ISNULL(src.IsAlternateLine, 0),
        ISNULL(src.IsMainLine, 0),
        ISNULL(src.IsStalePregamePrice, 0),
        ISNULL(src.IsPlayerProp, 0),
        ISNULL(src.IsImpossibleScoreline, 0),
        src.SeasonType,
        src.HomeId,
        src.HomeNumericalId,
        src.HomeName,
        src.HomeLogo,
        src.HomeAbbreviation,
        src.HomeCity,
        src.HomeMascot,
        src.HomeConference,
        src.HomeDivision,
        src.AwayId,
        src.AwayNumericalId,
        src.AwayName,
        src.AwayLogo,
        src.AwayAbbreviation,
        src.AwayCity,
        src.AwayMascot,
        src.AwayConference,
        src.AwayDivision,
        src.SportRefId,
        src.SportRefName,
        src.SportRefNumericalId,
        src.LeagueRefId,
        src.LeagueRefLabel,
        src.LeagueRefNumericalId,
        src.MarketRefId,
        src.MarketRefLabel,
        src.MarketRefNumericalId,
        src.SportsbookRefId,
        src.SportsbookRefName,
        src.SportsbookRefNumericalId,
        src.CompetitionRefId,
        src.CompetitionRefLabel,
        src.CompetitionRefNumericalId,
        src.ExternalEventId,
        SYSDATETIME()
    FROM OPENJSON(@Json)
    WITH
    (
        Id                       VARCHAR(64)    '$.id',
        Sportsbook               VARCHAR(128)   '$.sportsbook',
        EventId                  VARCHAR(256)   '$.eventId',
        EventUuid                VARCHAR(64)    '$.eventUuid',
        Sport                    VARCHAR(64)    '$.sport',
        League                   VARCHAR(256)   '$.league',
        HomeTeam                 VARCHAR(256)   '$.homeTeam',
        AwayTeam                 VARCHAR(256)   '$.awayTeam',
        MarketType               VARCHAR(128)   '$.marketType',
        Selection                VARCHAR(256)   '$.selection',
        SelectionType            VARCHAR(64)    '$.selectionType',
        TeamSide                 VARCHAR(64)    '$.teamSide',
        OddsAmerican             DECIMAL(18, 8)  '$.oddsAmerican',
        OddsDecimal              DECIMAL(18, 8)  '$.oddsDecimal',
        OddsProbability          DECIMAL(18, 8)  '$.oddsProbability',
        Line                     DECIMAL(18, 8)  '$.line',
        EventStartTime           DATETIME2       '$.eventStartTime',
        IsLive                   BIT             '$.isLive',
        IsActive                 BIT             '$.isActive',
        [Timestamp]              DATETIME2       '$.timestamp',
        DeepLink                 VARCHAR(512)   '$.deepLink',
        MarketId                 VARCHAR(128)   '$.marketId',
        SelectionId              VARCHAR(128)   '$.selectionId',
        ExternalMarketId         VARCHAR(128)   '$.externalMarketId',
        IsAlternateLine          BIT             '$.isAlternateLine',
        IsMainLine               BIT             '$.isMainLine',
        IsStalePregamePrice      BIT             '$.isStalePregamePrice',
        IsPlayerProp             BIT             '$.isPlayerProp',
        IsImpossibleScoreline    BIT             '$.isImpossibleScoreline',
        SeasonType               VARCHAR(64)    '$.seasonType',
        HomeId                   VARCHAR(128)   '$.homeId',
        HomeNumericalId          INT             '$.homeNumericalId',
        HomeName                 VARCHAR(256)   '$.homeName',
        HomeLogo                 VARCHAR(512)   '$.homeLogo',
        HomeAbbreviation         VARCHAR(32)    '$.homeAbbreviation',
        HomeCity                 VARCHAR(128)   '$.homeCity',
        HomeMascot               VARCHAR(128)   '$.homeMascot',
        HomeConference           VARCHAR(64)    '$.homeConference',
        HomeDivision             VARCHAR(128)   '$.homeDivision',
        AwayId                   VARCHAR(128)   '$.awayId',
        AwayNumericalId          INT             '$.awayNumericalId',
        AwayName                 VARCHAR(256)   '$.awayName',
        AwayLogo                 VARCHAR(512)   '$.awayLogo',
        AwayAbbreviation         VARCHAR(32)    '$.awayAbbreviation',
        AwayCity                 VARCHAR(128)   '$.awayCity',
        AwayMascot               VARCHAR(128)   '$.awayMascot',
        AwayConference           VARCHAR(64)    '$.awayConference',
        AwayDivision             VARCHAR(128)   '$.awayDivision',
        SportRefId               VARCHAR(64)    '$.sportRefId',
        SportRefName             VARCHAR(128)   '$.sportRefName',
        SportRefNumericalId      INT             '$.sportRefNumericalId',
        LeagueRefId              VARCHAR(256)   '$.leagueRefId',
        LeagueRefLabel           VARCHAR(256)   '$.leagueRefLabel',
        LeagueRefNumericalId     INT             '$.leagueRefNumericalId',
        MarketRefId              VARCHAR(128)   '$.marketRefId',
        MarketRefLabel           VARCHAR(128)   '$.marketRefLabel',
        MarketRefNumericalId     INT             '$.marketRefNumericalId',
        SportsbookRefId          VARCHAR(128)   '$.sportsbookRefId',
        SportsbookRefName        VARCHAR(128)   '$.sportsbookRefName',
        SportsbookRefNumericalId INT             '$.sportsbookRefNumericalId',
        CompetitionRefId         VARCHAR(256)   '$.competitionRefId',
        CompetitionRefLabel      VARCHAR(256)   '$.competitionRefLabel',
        CompetitionRefNumericalId INT            '$.competitionRefNumericalId',
        ExternalEventId          VARCHAR(64)    '$.externalEventId'
    ) AS src;

    DECLARE @OddCount INT = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT @OddCount AS OddCount;
END;
GO
```

### JSON de ejemplo para `spr_MarketOdds_BulkLoad`

```json
[
  {
    "id": "111683319829914",
    "sportsbook": "draftkings",
    "eventId": "tennis_men_lorenzo_giustino_yanaki_milev_2026-10-20",
    "eventUuid": "df922c32ff405150",
    "sport": "tennis",
    "league": "atp_challenger",
    "homeTeam": "Lorenzo Giustino",
    "awayTeam": "Yanaki Milev",
    "marketType": "moneyline",
    "selection": "Lorenzo Giustino",
    "selectionType": "home",
    "teamSide": "home",
    "oddsAmerican": 137,
    "oddsDecimal": 2.37,
    "oddsProbability": 0.4219,
    "line": null,
    "eventStartTime": "2026-10-20T14:50:00Z",
    "isLive": false,
    "isActive": true,
    "timestamp": "2026-10-20T20:10:37Z",
    "deepLink": "https://api.sharpapi.io/api/v1/deeplink/111683319829914",
    "marketId": "1_86843912",
    "selectionId": "0ML86843912_3",
    "externalMarketId": null,
    "isAlternateLine": false,
    "isMainLine": true,
    "isStalePregamePrice": false,
    "isPlayerProp": false,
    "isImpossibleScoreline": false,
    "seasonType": null,
    "homeId": "atp_lorenzo_giustino",
    "homeNumericalId": 1920,
    "homeName": "Lorenzo Giustino",
    "homeLogo": "https://cdn.sharpapi.io/teams/tennis/1920.png",
    "awayId": "atp_challenger_yanaki_milev",
    "awayNumericalId": 4457,
    "awayName": "Yanaki Milev",
    "awayLogo": "https://cdn.sharpapi.io/teams/tennis/4457.png",
    "competitionRefId": "atp_challenger_-_florence_italy_2026",
    "competitionRefLabel": "Florence, Italy",
    "competitionRefNumericalId": 1423,
    "externalEventId": "34864075"
  }
]
```

## Carga incremental de cuotas (`GET /odds/delta`)

Tabla nueva `MarketOddsDelta` (distinta de `MarketOdds`) y procedimiento `dbo.spr_MarketOddsDelta_DeltaLoad`. Lo dispara `POST /api/odds/delta/sync` con **solo** `OddsDelta` de `appsettings.json`. La API vacía `MarketOddsDelta` antes de la primera página. Recibe un JSON `{ "data": [...], "removed": [{ "id": "..." }] }`. MERGE upserta `data` (`CreatedAtUtc` solo en INSERT) y DELETE las filas cuyo `Id` está en `removed`. No toca `MarketOdds`.

Ejecutar contra la base `Deportes`. Si ya existía `dbo.spr_MarketOdds_DeltaLoad`, el script lo elimina para no seguir escribiendo en el snapshot.

```sql
/* -------------------------------------------------------------------------- */
/* Cuotas incrementales (GET /odds/delta) — tabla distinta de MarketOdds      */
/* -------------------------------------------------------------------------- */

IF OBJECT_ID(N'dbo.MarketOddsDelta', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MarketOddsDelta
    (
        Id                       VARCHAR(64)    NOT NULL CONSTRAINT PK_MarketOddsDelta PRIMARY KEY,
        Sportsbook               VARCHAR(128)   NOT NULL,
        EventId                  VARCHAR(256)   NOT NULL,
        EventUuid                VARCHAR(64)    NULL,
        Sport                    VARCHAR(64)    NOT NULL,
        League                   VARCHAR(256)   NOT NULL,
        HomeTeam                 VARCHAR(256)   NULL,
        AwayTeam                 VARCHAR(256)   NULL,
        MarketType               VARCHAR(128)   NOT NULL,
        Selection                VARCHAR(256)   NOT NULL,
        SelectionType            VARCHAR(64)    NULL,
        TeamSide                 VARCHAR(64)    NULL,
        OddsAmerican             DECIMAL(18, 8)  NULL,
        OddsDecimal              DECIMAL(18, 8)  NULL,
        OddsProbability          DECIMAL(18, 8)  NULL,
        Line                     DECIMAL(18, 8)  NULL,
        EventStartTime           DATETIME2       NULL,
        IsLive                   BIT             NOT NULL CONSTRAINT DF_MarketOddsDelta_IsLive DEFAULT (0),
        IsActive                 BIT             NOT NULL CONSTRAINT DF_MarketOddsDelta_IsActive DEFAULT (1),
        [Timestamp]              DATETIME2       NULL,
        DeepLink                 VARCHAR(512)   NULL,
        MarketId                 VARCHAR(128)   NULL,
        SelectionId              VARCHAR(128)   NULL,
        ExternalMarketId         VARCHAR(128)   NULL,
        IsAlternateLine          BIT             NOT NULL CONSTRAINT DF_MarketOddsDelta_IsAlternateLine DEFAULT (0),
        IsMainLine               BIT             NOT NULL CONSTRAINT DF_MarketOddsDelta_IsMainLine DEFAULT (0),
        IsStalePregamePrice      BIT             NOT NULL CONSTRAINT DF_MarketOddsDelta_IsStalePregamePrice DEFAULT (0),
        IsPlayerProp             BIT             NOT NULL CONSTRAINT DF_MarketOddsDelta_IsPlayerProp DEFAULT (0),
        IsImpossibleScoreline    BIT             NOT NULL CONSTRAINT DF_MarketOddsDelta_IsImpossibleScoreline DEFAULT (0),
        SeasonType               VARCHAR(64)    NULL,
        HomeId                   VARCHAR(128)   NULL,
        HomeNumericalId          INT             NULL,
        HomeName                 VARCHAR(256)   NULL,
        HomeLogo                 VARCHAR(512)   NULL,
        HomeAbbreviation         VARCHAR(32)    NULL,
        HomeCity                 VARCHAR(128)   NULL,
        HomeMascot               VARCHAR(128)   NULL,
        HomeConference           VARCHAR(64)    NULL,
        HomeDivision             VARCHAR(128)   NULL,
        AwayId                   VARCHAR(128)   NULL,
        AwayNumericalId          INT             NULL,
        AwayName                 VARCHAR(256)   NULL,
        AwayLogo                 VARCHAR(512)   NULL,
        AwayAbbreviation         VARCHAR(32)    NULL,
        AwayCity                 VARCHAR(128)   NULL,
        AwayMascot               VARCHAR(128)   NULL,
        AwayConference           VARCHAR(64)    NULL,
        AwayDivision             VARCHAR(128)   NULL,
        SportRefId               VARCHAR(64)    NULL,
        SportRefName             VARCHAR(128)   NULL,
        SportRefNumericalId      INT             NULL,
        LeagueRefId              VARCHAR(256)   NULL,
        LeagueRefLabel           VARCHAR(256)   NULL,
        LeagueRefNumericalId     INT             NULL,
        MarketRefId              VARCHAR(128)   NULL,
        MarketRefLabel           VARCHAR(128)   NULL,
        MarketRefNumericalId     INT             NULL,
        SportsbookRefId          VARCHAR(128)   NULL,
        SportsbookRefName        VARCHAR(128)   NULL,
        SportsbookRefNumericalId INT             NULL,
        CompetitionRefId         VARCHAR(256)   NULL,
        CompetitionRefLabel      VARCHAR(256)   NULL,
        CompetitionRefNumericalId INT            NULL,
        ExternalEventId          VARCHAR(64)    NULL,
        CreatedAtUtc             DATETIME2       NOT NULL CONSTRAINT DF_MarketOddsDelta_CreatedAtUtc DEFAULT (SYSDATETIME())
    );
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOddsDelta_Sport' AND object_id = OBJECT_ID(N'dbo.MarketOddsDelta'))
BEGIN
    CREATE INDEX IX_MarketOddsDelta_Sport ON dbo.MarketOddsDelta (Sport);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOddsDelta_EventId' AND object_id = OBJECT_ID(N'dbo.MarketOddsDelta'))
BEGIN
    CREATE INDEX IX_MarketOddsDelta_EventId ON dbo.MarketOddsDelta (EventId);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOddsDelta_League' AND object_id = OBJECT_ID(N'dbo.MarketOddsDelta'))
BEGIN
    CREATE INDEX IX_MarketOddsDelta_League ON dbo.MarketOddsDelta (League);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOddsDelta_MarketType' AND object_id = OBJECT_ID(N'dbo.MarketOddsDelta'))
BEGIN
    CREATE INDEX IX_MarketOddsDelta_MarketType ON dbo.MarketOddsDelta (MarketType);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOddsDelta_Sportsbook' AND object_id = OBJECT_ID(N'dbo.MarketOddsDelta'))
BEGIN
    CREATE INDEX IX_MarketOddsDelta_Sportsbook ON dbo.MarketOddsDelta (Sportsbook);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOddsDelta_EventStartTime' AND object_id = OBJECT_ID(N'dbo.MarketOddsDelta'))
BEGIN
    CREATE INDEX IX_MarketOddsDelta_EventStartTime ON dbo.MarketOddsDelta (EventStartTime);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_MarketOddsDelta_CreatedAtUtc' AND object_id = OBJECT_ID(N'dbo.MarketOddsDelta'))
BEGIN
    CREATE INDEX IX_MarketOddsDelta_CreatedAtUtc ON dbo.MarketOddsDelta (CreatedAtUtc);
END
GO

IF OBJECT_ID(N'dbo.MarketOddsDelta', N'U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'dbo.DF_MarketOddsDelta_CreatedAtUtc', N'D') IS NOT NULL
        ALTER TABLE dbo.MarketOddsDelta DROP CONSTRAINT DF_MarketOddsDelta_CreatedAtUtc;

    ALTER TABLE dbo.MarketOddsDelta
        ADD CONSTRAINT DF_MarketOddsDelta_CreatedAtUtc DEFAULT (SYSDATETIME()) FOR CreatedAtUtc;
END
GO

IF OBJECT_ID(N'dbo.spr_MarketOdds_DeltaLoad', N'P') IS NOT NULL
    DROP PROCEDURE dbo.spr_MarketOdds_DeltaLoad;
GO

CREATE OR ALTER PROCEDURE dbo.spr_MarketOddsDelta_DeltaLoad
    @Json VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Json IS NULL OR LTRIM(RTRIM(@Json)) = N''
    BEGIN
        SELECT 0 AS UpsertedCount, 0 AS DeletedCount;
        RETURN;
    END;

    DECLARE @UpsertedCount INT = 0;
    DECLARE @DeletedCount INT = 0;

    BEGIN TRANSACTION;

    ;WITH Parsed AS
    (
        SELECT
            src.Id,
            ISNULL(src.Sportsbook, N'') AS Sportsbook,
            ISNULL(src.EventId, N'') AS EventId,
            src.EventUuid,
            ISNULL(src.Sport, N'') AS Sport,
            ISNULL(src.League, N'') AS League,
            src.HomeTeam,
            src.AwayTeam,
            ISNULL(src.MarketType, N'') AS MarketType,
            ISNULL(src.Selection, N'') AS Selection,
            src.SelectionType,
            src.TeamSide,
            src.OddsAmerican,
            src.OddsDecimal,
            src.OddsProbability,
            src.Line,
            src.EventStartTime,
            ISNULL(src.IsLive, 0) AS IsLive,
            ISNULL(src.IsActive, 1) AS IsActive,
            src.[Timestamp],
            src.DeepLink,
            src.MarketId,
            src.SelectionId,
            src.ExternalMarketId,
            ISNULL(src.IsAlternateLine, 0) AS IsAlternateLine,
            ISNULL(src.IsMainLine, 0) AS IsMainLine,
            ISNULL(src.IsStalePregamePrice, 0) AS IsStalePregamePrice,
            ISNULL(src.IsPlayerProp, 0) AS IsPlayerProp,
            ISNULL(src.IsImpossibleScoreline, 0) AS IsImpossibleScoreline,
            src.SeasonType,
            src.HomeId,
            src.HomeNumericalId,
            src.HomeName,
            src.HomeLogo,
            src.HomeAbbreviation,
            src.HomeCity,
            src.HomeMascot,
            src.HomeConference,
            src.HomeDivision,
            src.AwayId,
            src.AwayNumericalId,
            src.AwayName,
            src.AwayLogo,
            src.AwayAbbreviation,
            src.AwayCity,
            src.AwayMascot,
            src.AwayConference,
            src.AwayDivision,
            src.SportRefId,
            src.SportRefName,
            src.SportRefNumericalId,
            src.LeagueRefId,
            src.LeagueRefLabel,
            src.LeagueRefNumericalId,
            src.MarketRefId,
            src.MarketRefLabel,
            src.MarketRefNumericalId,
            src.SportsbookRefId,
            src.SportsbookRefName,
            src.SportsbookRefNumericalId,
            src.CompetitionRefId,
            src.CompetitionRefLabel,
            src.CompetitionRefNumericalId,
            src.ExternalEventId,
            ROW_NUMBER() OVER (PARTITION BY src.Id ORDER BY (SELECT NULL)) AS Rn
        FROM OPENJSON(@Json, '$.data')
        WITH
        (
            Id                       VARCHAR(64)    '$.id',
            Sportsbook               VARCHAR(128)   '$.sportsbook',
            EventId                  VARCHAR(256)   '$.eventId',
            EventUuid                VARCHAR(64)    '$.eventUuid',
            Sport                    VARCHAR(64)    '$.sport',
            League                   VARCHAR(256)   '$.league',
            HomeTeam                 VARCHAR(256)   '$.homeTeam',
            AwayTeam                 VARCHAR(256)   '$.awayTeam',
            MarketType               VARCHAR(128)   '$.marketType',
            Selection                VARCHAR(256)   '$.selection',
            SelectionType            VARCHAR(64)    '$.selectionType',
            TeamSide                 VARCHAR(64)    '$.teamSide',
            OddsAmerican             DECIMAL(18, 8)  '$.oddsAmerican',
            OddsDecimal              DECIMAL(18, 8)  '$.oddsDecimal',
            OddsProbability          DECIMAL(18, 8)  '$.oddsProbability',
            Line                     DECIMAL(18, 8)  '$.line',
            EventStartTime           DATETIME2       '$.eventStartTime',
            IsLive                   BIT             '$.isLive',
            IsActive                 BIT             '$.isActive',
            [Timestamp]              DATETIME2       '$.timestamp',
            DeepLink                 VARCHAR(512)   '$.deepLink',
            MarketId                 VARCHAR(128)   '$.marketId',
            SelectionId              VARCHAR(128)   '$.selectionId',
            ExternalMarketId         VARCHAR(128)   '$.externalMarketId',
            IsAlternateLine          BIT             '$.isAlternateLine',
            IsMainLine               BIT             '$.isMainLine',
            IsStalePregamePrice      BIT             '$.isStalePregamePrice',
            IsPlayerProp             BIT             '$.isPlayerProp',
            IsImpossibleScoreline    BIT             '$.isImpossibleScoreline',
            SeasonType               VARCHAR(64)    '$.seasonType',
            HomeId                   VARCHAR(128)   '$.homeId',
            HomeNumericalId          INT             '$.homeNumericalId',
            HomeName                 VARCHAR(256)   '$.homeName',
            HomeLogo                 VARCHAR(512)   '$.homeLogo',
            HomeAbbreviation         VARCHAR(32)    '$.homeAbbreviation',
            HomeCity                 VARCHAR(128)   '$.homeCity',
            HomeMascot               VARCHAR(128)   '$.homeMascot',
            HomeConference           VARCHAR(64)    '$.homeConference',
            HomeDivision             VARCHAR(128)   '$.homeDivision',
            AwayId                   VARCHAR(128)   '$.awayId',
            AwayNumericalId          INT             '$.awayNumericalId',
            AwayName                 VARCHAR(256)   '$.awayName',
            AwayLogo                 VARCHAR(512)   '$.awayLogo',
            AwayAbbreviation         VARCHAR(32)    '$.awayAbbreviation',
            AwayCity                 VARCHAR(128)   '$.awayCity',
            AwayMascot               VARCHAR(128)   '$.awayMascot',
            AwayConference           VARCHAR(64)    '$.awayConference',
            AwayDivision             VARCHAR(128)   '$.awayDivision',
            SportRefId               VARCHAR(64)    '$.sportRefId',
            SportRefName             VARCHAR(128)   '$.sportRefName',
            SportRefNumericalId      INT             '$.sportRefNumericalId',
            LeagueRefId              VARCHAR(256)   '$.leagueRefId',
            LeagueRefLabel           VARCHAR(256)   '$.leagueRefLabel',
            LeagueRefNumericalId     INT             '$.leagueRefNumericalId',
            MarketRefId              VARCHAR(128)   '$.marketRefId',
            MarketRefLabel           VARCHAR(128)   '$.marketRefLabel',
            MarketRefNumericalId     INT             '$.marketRefNumericalId',
            SportsbookRefId          VARCHAR(128)   '$.sportsbookRefId',
            SportsbookRefName        VARCHAR(128)   '$.sportsbookRefName',
            SportsbookRefNumericalId INT             '$.sportsbookRefNumericalId',
            CompetitionRefId         VARCHAR(256)   '$.competitionRefId',
            CompetitionRefLabel      VARCHAR(256)   '$.competitionRefLabel',
            CompetitionRefNumericalId INT            '$.competitionRefNumericalId',
            ExternalEventId          VARCHAR(64)    '$.externalEventId'
        ) AS src
        WHERE NULLIF(src.Id, N'') IS NOT NULL
    )
    MERGE dbo.MarketOddsDelta AS target
    USING (SELECT * FROM Parsed WHERE Rn = 1) AS source
    ON target.Id = source.Id
    WHEN MATCHED THEN UPDATE SET
        Sportsbook = source.Sportsbook,
        EventId = source.EventId,
        EventUuid = source.EventUuid,
        Sport = source.Sport,
        League = source.League,
        HomeTeam = source.HomeTeam,
        AwayTeam = source.AwayTeam,
        MarketType = source.MarketType,
        Selection = source.Selection,
        SelectionType = source.SelectionType,
        TeamSide = source.TeamSide,
        OddsAmerican = source.OddsAmerican,
        OddsDecimal = source.OddsDecimal,
        OddsProbability = source.OddsProbability,
        Line = source.Line,
        EventStartTime = source.EventStartTime,
        IsLive = source.IsLive,
        IsActive = source.IsActive,
        [Timestamp] = source.[Timestamp],
        DeepLink = source.DeepLink,
        MarketId = source.MarketId,
        SelectionId = source.SelectionId,
        ExternalMarketId = source.ExternalMarketId,
        IsAlternateLine = source.IsAlternateLine,
        IsMainLine = source.IsMainLine,
        IsStalePregamePrice = source.IsStalePregamePrice,
        IsPlayerProp = source.IsPlayerProp,
        IsImpossibleScoreline = source.IsImpossibleScoreline,
        SeasonType = source.SeasonType,
        HomeId = source.HomeId,
        HomeNumericalId = source.HomeNumericalId,
        HomeName = source.HomeName,
        HomeLogo = source.HomeLogo,
        HomeAbbreviation = source.HomeAbbreviation,
        HomeCity = source.HomeCity,
        HomeMascot = source.HomeMascot,
        HomeConference = source.HomeConference,
        HomeDivision = source.HomeDivision,
        AwayId = source.AwayId,
        AwayNumericalId = source.AwayNumericalId,
        AwayName = source.AwayName,
        AwayLogo = source.AwayLogo,
        AwayAbbreviation = source.AwayAbbreviation,
        AwayCity = source.AwayCity,
        AwayMascot = source.AwayMascot,
        AwayConference = source.AwayConference,
        AwayDivision = source.AwayDivision,
        SportRefId = source.SportRefId,
        SportRefName = source.SportRefName,
        SportRefNumericalId = source.SportRefNumericalId,
        LeagueRefId = source.LeagueRefId,
        LeagueRefLabel = source.LeagueRefLabel,
        LeagueRefNumericalId = source.LeagueRefNumericalId,
        MarketRefId = source.MarketRefId,
        MarketRefLabel = source.MarketRefLabel,
        MarketRefNumericalId = source.MarketRefNumericalId,
        SportsbookRefId = source.SportsbookRefId,
        SportsbookRefName = source.SportsbookRefName,
        SportsbookRefNumericalId = source.SportsbookRefNumericalId,
        CompetitionRefId = source.CompetitionRefId,
        CompetitionRefLabel = source.CompetitionRefLabel,
        CompetitionRefNumericalId = source.CompetitionRefNumericalId,
        ExternalEventId = source.ExternalEventId
    WHEN NOT MATCHED THEN INSERT
    (
        Id, Sportsbook, EventId, EventUuid, Sport, League, HomeTeam, AwayTeam,
        MarketType, Selection, SelectionType, TeamSide,
        OddsAmerican, OddsDecimal, OddsProbability, Line, EventStartTime,
        IsLive, IsActive, [Timestamp], DeepLink, MarketId, SelectionId, ExternalMarketId,
        IsAlternateLine, IsMainLine, IsStalePregamePrice, IsPlayerProp, IsImpossibleScoreline,
        SeasonType,
        HomeId, HomeNumericalId, HomeName, HomeLogo, HomeAbbreviation, HomeCity, HomeMascot, HomeConference, HomeDivision,
        AwayId, AwayNumericalId, AwayName, AwayLogo, AwayAbbreviation, AwayCity, AwayMascot, AwayConference, AwayDivision,
        SportRefId, SportRefName, SportRefNumericalId,
        LeagueRefId, LeagueRefLabel, LeagueRefNumericalId,
        MarketRefId, MarketRefLabel, MarketRefNumericalId,
        SportsbookRefId, SportsbookRefName, SportsbookRefNumericalId,
        CompetitionRefId, CompetitionRefLabel, CompetitionRefNumericalId,
        ExternalEventId, CreatedAtUtc
    )
    VALUES
    (
        source.Id, source.Sportsbook, source.EventId, source.EventUuid, source.Sport, source.League, source.HomeTeam, source.AwayTeam,
        source.MarketType, source.Selection, source.SelectionType, source.TeamSide,
        source.OddsAmerican, source.OddsDecimal, source.OddsProbability, source.Line, source.EventStartTime,
        source.IsLive, source.IsActive, source.[Timestamp], source.DeepLink, source.MarketId, source.SelectionId, source.ExternalMarketId,
        source.IsAlternateLine, source.IsMainLine, source.IsStalePregamePrice, source.IsPlayerProp, source.IsImpossibleScoreline,
        source.SeasonType,
        source.HomeId, source.HomeNumericalId, source.HomeName, source.HomeLogo, source.HomeAbbreviation, source.HomeCity, source.HomeMascot, source.HomeConference, source.HomeDivision,
        source.AwayId, source.AwayNumericalId, source.AwayName, source.AwayLogo, source.AwayAbbreviation, source.AwayCity, source.AwayMascot, source.AwayConference, source.AwayDivision,
        source.SportRefId, source.SportRefName, source.SportRefNumericalId,
        source.LeagueRefId, source.LeagueRefLabel, source.LeagueRefNumericalId,
        source.MarketRefId, source.MarketRefLabel, source.MarketRefNumericalId,
        source.SportsbookRefId, source.SportsbookRefName, source.SportsbookRefNumericalId,
        source.CompetitionRefId, source.CompetitionRefLabel, source.CompetitionRefNumericalId,
        source.ExternalEventId, SYSDATETIME()
    );

    SET @UpsertedCount = @@ROWCOUNT;

    DELETE FROM dbo.MarketOddsDelta
    WHERE Id IN
    (
        SELECT removed.Id
        FROM OPENJSON(@Json, '$.removed')
        WITH (Id VARCHAR(64) '$.id') AS removed
        WHERE NULLIF(removed.Id, N'') IS NOT NULL
    );

    SET @DeletedCount = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT @UpsertedCount AS UpsertedCount, @DeletedCount AS DeletedCount;
END;
GO
```

### JSON de ejemplo para `spr_MarketOddsDelta_DeltaLoad`

```json
{
  "data": [
    {
      "id": "100237696465956",
      "sportsbook": "bwin",
      "eventId": "table_tennis_-_tt_cup_moravecpetr_pavelrissky_2026-10-28_b3m1930",
      "eventUuid": "1d285b225cbfcb83",
      "sport": "table_tennis",
      "league": "table_tennis_-_tt_cup",
      "homeTeam": "Petr Moravec",
      "awayTeam": "Pavel Rissky",
      "marketType": "point_spread",
      "selection": "Petr Moravec",
      "selectionType": "home",
      "teamSide": "home",
      "oddsAmerican": -108,
      "oddsDecimal": 1.926,
      "oddsProbability": 0.5192,
      "line": 10.5,
      "eventStartTime": "2026-10-28T19:30:00Z",
      "isLive": true,
      "isActive": true,
      "timestamp": "2026-10-28T19:47:29.2850241Z",
      "deepLink": "https://api.sharpapi.io/api/v1/deeplink/100237696465956",
      "marketId": "71661882",
      "selectionId": "172893308",
      "isAlternateLine": true,
      "isMainLine": false,
      "isStalePregamePrice": false,
      "isPlayerProp": false,
      "isImpossibleScoreline": false,
      "homeId": "table_tennis_petr_moravec",
      "homeNumericalId": 28010,
      "homeName": "Petr Moravec",
      "sportRefId": "table_tennis",
      "sportRefName": "Table Tennis",
      "sportRefNumericalId": 27,
      "leagueRefId": "table_tennis_-_tt_cup",
      "leagueRefLabel": "TT Cup",
      "leagueRefNumericalId": 527,
      "marketRefId": "point_spread",
      "marketRefLabel": "Point Spread",
      "marketRefNumericalId": 1117,
      "sportsbookRefId": "bwin",
      "sportsbookRefName": "bwin",
      "sportsbookRefNumericalId": 48,
      "externalEventId": "5:1329446"
    }
  ],
  "removed": [
    {
      "id": "1070039450443613196"
    }
  ]
}
```

## Oportunidades de arbitraje SharpAPI (`GET /opportunities/arbitrage`)

Tablas distintas de `MarketOdds` y de los SP `spr_MarketOdds_Arbitrage*`. Aquí se **guarda** lo que SharpAPI ya detectó; no se calcula el arb en SQL.

Lo dispara `POST /api/arbitrage/sync`. Recibe un JSON (array de oportunidades con `legs`) y `@Sportsbooks` (casas permitidas, CSV: `bwin,betsson`). Solo inserta si **todas** las patas son de esa lista (casa A y casa B; si hay 3 patas, las 3). Si `@Sportsbooks` va vacío, no filtra por casa. No reinserta un `Id` que ya exista (así se conservan las filas con menos de `ArbitrageFeed:RetentionMinutes`).

`CreatedAtUtc` guarda la fecha y hora local de inserción. El sync borra solo las filas con edad **mayor o igual** a `RetentionMinutes` (por defecto 3). Las más jóvenes se quedan.

Ejecutar contra la base `Deportes` después de crear las tablas. Si la tabla ya existe, ejecuta también el `ALTER` de `CreatedAtUtc`.

```sql
USE [Deportes];
GO

IF OBJECT_ID(N'dbo.ArbitrageOpportunities', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ArbitrageOpportunities
    (
        Id                         VARCHAR(64)    NOT NULL CONSTRAINT PK_ArbitrageOpportunities PRIMARY KEY,
        DetectedAt                 DATETIME2       NULL,
        EstimatedNetProfitPercent  DECIMAL(18, 8)  NULL,
        EvAvailable                BIT             NOT NULL CONSTRAINT DF_ArbitrageOpportunities_EvAvailable DEFAULT (0),
        EvPercentage               DECIMAL(18, 8)  NULL,
        EventId                    VARCHAR(256)   NOT NULL,
        EventName                  VARCHAR(512)   NOT NULL,
        ImpliedTotal               DECIMAL(18, 8)  NULL,
        IsAlternateLine            BIT             NOT NULL CONSTRAINT DF_ArbitrageOpportunities_IsAlternateLine DEFAULT (0),
        IsLive                     BIT             NOT NULL CONSTRAINT DF_ArbitrageOpportunities_IsLive DEFAULT (0),
        IsPlayerProp               BIT             NOT NULL CONSTRAINT DF_ArbitrageOpportunities_IsPlayerProp DEFAULT (0),
        League                     VARCHAR(256)   NOT NULL,
        LeagueLabel                VARCHAR(256)   NULL,
        Line                       DECIMAL(18, 8)  NULL,
        LineUnchanged              BIT             NOT NULL CONSTRAINT DF_ArbitrageOpportunities_LineUnchanged DEFAULT (0),
        MarketLabel                VARCHAR(256)   NULL,
        MarketType                 VARCHAR(128)   NOT NULL,
        OldestOddsAgeSeconds       DECIMAL(18, 8)  NULL,
        PlayerName                 VARCHAR(256)   NULL,
        PossiblyStale              BIT             NOT NULL CONSTRAINT DF_ArbitrageOpportunities_PossiblyStale DEFAULT (0),
        ProfitPercent              DECIMAL(18, 8)  NULL,
        Sport                      VARCHAR(64)    NOT NULL,
        StartTime                  DATETIME2       NULL,
        StatCategory               VARCHAR(128)   NULL,
        Warnings                   VARCHAR(1024)  NULL,
        CreatedAtUtc               DATETIME2       NOT NULL CONSTRAINT DF_ArbitrageOpportunities_CreatedAtUtc DEFAULT (SYSDATETIME())
    );
END
GO

IF COL_LENGTH(N'dbo.ArbitrageOpportunities', N'CreatedAtUtc') IS NULL
BEGIN
    ALTER TABLE dbo.ArbitrageOpportunities
        ADD CreatedAtUtc DATETIME2 NOT NULL
            CONSTRAINT DF_ArbitrageOpportunities_CreatedAtUtc DEFAULT (SYSDATETIME());
END
GO

IF OBJECT_ID(N'dbo.ArbitrageOpportunities', N'U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'dbo.DF_ArbitrageOpportunities_CreatedAtUtc', N'D') IS NOT NULL
        ALTER TABLE dbo.ArbitrageOpportunities DROP CONSTRAINT DF_ArbitrageOpportunities_CreatedAtUtc;

    ALTER TABLE dbo.ArbitrageOpportunities
        ADD CONSTRAINT DF_ArbitrageOpportunities_CreatedAtUtc DEFAULT (SYSDATETIME()) FOR CreatedAtUtc;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ArbitrageOpportunities_Sport' AND object_id = OBJECT_ID(N'dbo.ArbitrageOpportunities'))
BEGIN
    CREATE INDEX IX_ArbitrageOpportunities_Sport ON dbo.ArbitrageOpportunities (Sport);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ArbitrageOpportunities_EventId' AND object_id = OBJECT_ID(N'dbo.ArbitrageOpportunities'))
BEGIN
    CREATE INDEX IX_ArbitrageOpportunities_EventId ON dbo.ArbitrageOpportunities (EventId);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ArbitrageOpportunities_League' AND object_id = OBJECT_ID(N'dbo.ArbitrageOpportunities'))
BEGIN
    CREATE INDEX IX_ArbitrageOpportunities_League ON dbo.ArbitrageOpportunities (League);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ArbitrageOpportunities_MarketType' AND object_id = OBJECT_ID(N'dbo.ArbitrageOpportunities'))
BEGIN
    CREATE INDEX IX_ArbitrageOpportunities_MarketType ON dbo.ArbitrageOpportunities (MarketType);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ArbitrageOpportunities_ProfitPercent' AND object_id = OBJECT_ID(N'dbo.ArbitrageOpportunities'))
BEGIN
    CREATE INDEX IX_ArbitrageOpportunities_ProfitPercent ON dbo.ArbitrageOpportunities (ProfitPercent);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ArbitrageOpportunities_CreatedAtUtc' AND object_id = OBJECT_ID(N'dbo.ArbitrageOpportunities'))
BEGIN
    CREATE INDEX IX_ArbitrageOpportunities_CreatedAtUtc ON dbo.ArbitrageOpportunities (CreatedAtUtc);
END
GO

IF OBJECT_ID(N'dbo.ArbitrageOpportunityLegs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ArbitrageOpportunityLegs
    (
        Id                 VARCHAR(64)    NOT NULL CONSTRAINT PK_ArbitrageOpportunityLegs PRIMARY KEY,
        ArbitrageId        VARCHAR(64)    NOT NULL,
        DeepLink           VARCHAR(1024)  NULL,
        ExternalEventId    VARCHAR(256)   NULL,
        ExternalMarketId   VARCHAR(256)   NULL,
        Line               DECIMAL(18, 8)  NULL,
        MarketId           VARCHAR(256)   NULL,
        OddsAmerican       DECIMAL(18, 8)  NULL,
        OddsDecimal        DECIMAL(18, 8)  NULL,
        OddsId             VARCHAR(64)    NULL,
        OddsProbability    DECIMAL(18, 8)  NULL,
        Selection          VARCHAR(512)   NOT NULL,
        SelectionId        VARCHAR(256)   NULL,
        Sportsbook         VARCHAR(128)   NOT NULL,
        StakePercent       DECIMAL(18, 8)  NULL,
        CONSTRAINT FK_ArbitrageOpportunityLegs_ArbitrageOpportunities_ArbitrageId
            FOREIGN KEY (ArbitrageId) REFERENCES dbo.ArbitrageOpportunities (Id) ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ArbitrageOpportunityLegs_ArbitrageId' AND object_id = OBJECT_ID(N'dbo.ArbitrageOpportunityLegs'))
BEGIN
    CREATE INDEX IX_ArbitrageOpportunityLegs_ArbitrageId ON dbo.ArbitrageOpportunityLegs (ArbitrageId);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ArbitrageOpportunityLegs_Sportsbook' AND object_id = OBJECT_ID(N'dbo.ArbitrageOpportunityLegs'))
BEGIN
    CREATE INDEX IX_ArbitrageOpportunityLegs_Sportsbook ON dbo.ArbitrageOpportunityLegs (Sportsbook);
END
GO

CREATE OR ALTER PROCEDURE dbo.spr_ArbitrageOpportunities_BulkLoad
    @Json        VARCHAR(MAX),
    @Sportsbooks VARCHAR(8000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Json IS NULL OR LTRIM(RTRIM(@Json)) IN ('', '[]')
    BEGIN
        SELECT 0 AS OpportunityCount, 0 AS LegCount;
        RETURN;
    END;

    DECLARE @Allowed TABLE
    (
        Sportsbook VARCHAR(128) NOT NULL PRIMARY KEY
    );

    IF @Sportsbooks IS NOT NULL AND LTRIM(RTRIM(@Sportsbooks)) <> ''
    BEGIN
        INSERT INTO @Allowed (Sportsbook)
        SELECT DISTINCT LOWER(LTRIM(RTRIM(value)))
        FROM STRING_SPLIT(@Sportsbooks, ',')
        WHERE LTRIM(RTRIM(value)) <> '';
    END;

    DECLARE @AllowedCount INT = (SELECT COUNT(*) FROM @Allowed);

    IF OBJECT_ID('tempdb..#Eligible') IS NOT NULL
        DROP TABLE #Eligible;

    SELECT
        p.Id
    INTO #Eligible
    FROM OPENJSON(@Json)
    WITH
    (
        Id   VARCHAR(64)     '$.id',
        Legs NVARCHAR(MAX)   '$.legs' AS JSON
    ) AS p
    CROSS APPLY OPENJSON(p.Legs)
    WITH
    (
        Sportsbook VARCHAR(128) '$.sportsbook'
    ) AS leg
    LEFT JOIN @Allowed AS a
        ON a.Sportsbook = LOWER(LTRIM(RTRIM(ISNULL(leg.Sportsbook, ''))))
    WHERE p.Id IS NOT NULL
      AND NULLIF(p.Id, '') IS NOT NULL
      AND NOT EXISTS (
            SELECT 1
            FROM dbo.ArbitrageOpportunities AS existing
            WHERE existing.Id = p.Id
        )
    GROUP BY p.Id
    HAVING COUNT(*) >= 2
       AND (
            @AllowedCount = 0
            OR SUM(CASE WHEN a.Sportsbook IS NOT NULL THEN 1 ELSE 0 END) = COUNT(*)
           );

    BEGIN TRANSACTION;

    INSERT INTO dbo.ArbitrageOpportunities
    (
        Id, DetectedAt, EstimatedNetProfitPercent, EvAvailable, EvPercentage,
        EventId, EventName, ImpliedTotal, IsAlternateLine, IsLive, IsPlayerProp,
        League, LeagueLabel, Line, LineUnchanged, MarketLabel, MarketType,
        OldestOddsAgeSeconds, PlayerName, PossiblyStale, ProfitPercent,
        Sport, StartTime, StatCategory, Warnings, CreatedAtUtc
    )
    SELECT
        ISNULL(NULLIF(src.Id, ''), REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', '')),
        src.DetectedAt,
        src.EstimatedNetProfitPercent,
        ISNULL(src.EvAvailable, 0),
        src.EvPercentage,
        ISNULL(src.EventId, ''),
        ISNULL(src.EventName, ''),
        src.ImpliedTotal,
        ISNULL(src.IsAlternateLine, 0),
        ISNULL(src.IsLive, 0),
        ISNULL(src.IsPlayerProp, 0),
        ISNULL(src.League, ''),
        src.LeagueLabel,
        src.Line,
        ISNULL(src.LineUnchanged, 0),
        src.MarketLabel,
        ISNULL(src.MarketType, ''),
        src.OldestOddsAgeSeconds,
        src.PlayerName,
        ISNULL(src.PossiblyStale, 0),
        src.ProfitPercent,
        ISNULL(src.Sport, ''),
        src.StartTime,
        src.StatCategory,
        src.Warnings,
        SYSDATETIME()
    FROM OPENJSON(@Json)
    WITH
    (
        Id                        VARCHAR(64)    '$.id',
        DetectedAt                DATETIME2       '$.detectedAt',
        EstimatedNetProfitPercent DECIMAL(18, 8)  '$.estimatedNetProfitPercent',
        EvAvailable               BIT             '$.evAvailable',
        EvPercentage              DECIMAL(18, 8)  '$.evPercentage',
        EventId                   VARCHAR(256)   '$.eventId',
        EventName                 VARCHAR(512)   '$.eventName',
        ImpliedTotal              DECIMAL(18, 8)  '$.impliedTotal',
        IsAlternateLine           BIT             '$.isAlternateLine',
        IsLive                    BIT             '$.isLive',
        IsPlayerProp              BIT             '$.isPlayerProp',
        League                    VARCHAR(256)   '$.league',
        LeagueLabel               VARCHAR(256)   '$.leagueLabel',
        Line                      DECIMAL(18, 8)  '$.line',
        LineUnchanged             BIT             '$.lineUnchanged',
        MarketLabel               VARCHAR(256)   '$.marketLabel',
        MarketType                VARCHAR(128)   '$.marketType',
        OldestOddsAgeSeconds      DECIMAL(18, 8)  '$.oldestOddsAgeSeconds',
        PlayerName                VARCHAR(256)   '$.playerName',
        PossiblyStale             BIT             '$.possiblyStale',
        ProfitPercent             DECIMAL(18, 8)  '$.profitPercent',
        Sport                     VARCHAR(64)    '$.sport',
        StartTime                 DATETIME2       '$.startTime',
        StatCategory              VARCHAR(128)   '$.statCategory',
        Warnings                  VARCHAR(1024)  '$.warnings'
    ) AS src
    INNER JOIN #Eligible AS e ON e.Id = src.Id;

    DECLARE @OpportunityCount INT = @@ROWCOUNT;

    INSERT INTO dbo.ArbitrageOpportunityLegs
    (
        Id, ArbitrageId, DeepLink, ExternalEventId, ExternalMarketId, Line,
        MarketId, OddsAmerican, OddsDecimal, OddsId, OddsProbability,
        Selection, SelectionId, Sportsbook, StakePercent
    )
    SELECT
        ISNULL(NULLIF(leg.Id, ''), REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', '')),
        parent.Id,
        leg.DeepLink,
        leg.ExternalEventId,
        leg.ExternalMarketId,
        leg.Line,
        leg.MarketId,
        leg.OddsAmerican,
        leg.OddsDecimal,
        leg.OddsId,
        leg.OddsProbability,
        ISNULL(leg.Selection, ''),
        leg.SelectionId,
        ISNULL(leg.Sportsbook, ''),
        leg.StakePercent
    FROM OPENJSON(@Json)
    WITH
    (
        Id   VARCHAR(64)     '$.id',
        -- OPENJSON exige nvarchar(max) solo con AS JSON; no se persiste así en la tabla.
        Legs NVARCHAR(MAX)   '$.legs' AS JSON
    ) AS parent
    CROSS APPLY OPENJSON(parent.Legs)
    WITH
    (
        Id               VARCHAR(64)    '$.id',
        DeepLink         VARCHAR(1024)  '$.deepLink',
        ExternalEventId  VARCHAR(256)   '$.externalEventId',
        ExternalMarketId VARCHAR(256)   '$.externalMarketId',
        Line             DECIMAL(18, 8)  '$.line',
        MarketId         VARCHAR(256)   '$.marketId',
        OddsAmerican     DECIMAL(18, 8)  '$.oddsAmerican',
        OddsDecimal      DECIMAL(18, 8)  '$.oddsDecimal',
        OddsId           VARCHAR(64)    '$.oddsId',
        OddsProbability  DECIMAL(18, 8)  '$.oddsProbability',
        Selection        VARCHAR(512)   '$.selection',
        SelectionId      VARCHAR(256)   '$.selectionId',
        Sportsbook       VARCHAR(128)   '$.sportsbook',
        StakePercent     DECIMAL(18, 8)  '$.stakePercent'
    ) AS leg
    INNER JOIN #Eligible AS e ON e.Id = parent.Id
    WHERE parent.Id IS NOT NULL;

    DECLARE @LegCount INT = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT @OpportunityCount AS OpportunityCount, @LegCount AS LegCount;
END;
GO
```

### JSON de ejemplo para `spr_ArbitrageOpportunities_BulkLoad`

```json
[
  {
    "id": "748222253b7acd85",
    "detectedAt": "2026-10-27T18:07:09.062Z",
    "estimatedNetProfitPercent": 8.05,
    "evAvailable": false,
    "evPercentage": null,
    "eventId": "sweden_-_basketligan_lulea_umeabskt_2026-10-27_b3",
    "eventName": "BC Lulea @ Umea BSKT",
    "impliedTotal": 92.38,
    "isAlternateLine": false,
    "isLive": true,
    "isPlayerProp": false,
    "league": "sweden_-_basketligan",
    "leagueLabel": "Sweden - Basketligan",
    "line": null,
    "lineUnchanged": true,
    "marketLabel": "Race To 10 Points",
    "marketType": "race_to_10_points",
    "oldestOddsAgeSeconds": 61.39,
    "playerName": null,
    "possiblyStale": true,
    "profitPercent": 8.25,
    "sport": "basketball",
    "startTime": "2026-10-27T18:04:00Z",
    "statCategory": null,
    "warnings": "LIVE_HIGH_PROFIT_SUSPICIOUS,LIVE_GAME,LIVE_STALE_ODDS,LINE_UNCHANGED",
    "legs": [
      {
        "id": "leg1",
        "deepLink": "https://api.sharpapi.io/api/v1/deeplink/269741327401311?state=pa",
        "externalEventId": "20049364",
        "externalMarketId": "",
        "line": null,
        "marketId": "1571771052",
        "oddsAmerican": -275,
        "oddsDecimal": 1.364,
        "oddsId": "269741327401311",
        "oddsProbability": 0.7333,
        "selection": "BC Lulea",
        "selectionId": "2332922269",
        "sportsbook": "bwin",
        "stakePercent": 79.38
      },
      {
        "id": "leg2",
        "deepLink": "https://api.sharpapi.io/api/v1/deeplink/136699943421415?state=pa",
        "externalEventId": "20049364",
        "externalMarketId": "",
        "line": null,
        "marketId": "1571771052",
        "oddsAmerican": 425,
        "oddsDecimal": 5.25,
        "oddsId": "136699943421415",
        "oddsProbability": 0.1905,
        "selection": "Umea BSKT",
        "selectionId": "2332922268",
        "sportsbook": "betmgm",
        "stakePercent": 20.62
      }
    ]
  }
]
```

### Boletas de arbitraje SharpAPI — `spr_ArbitrageOpportunities`

Lee `ArbitrageOpportunities` + `ArbitrageOpportunityLegs` (lo ya sincronizado). Solo **2 patas**. Montos en múltiplos de 100 COP. Incluye `CreatedAtUtc`. `GET /api/arbitrage` no recibe query params: pasa `ArbitrageFeed` (sport, minProfit, isLive) y `ValueModel:BankrollCop`.

```sql
USE [Deportes];
GO

CREATE OR ALTER PROCEDURE dbo.spr_ArbitrageOpportunities
    @Bankroll         DECIMAL(18, 2) = 1000000,
    @Sport            VARCHAR(8000)  = NULL,
    @MinProfitPercent DECIMAL(18, 8) = 0,
    @IsLive           BIT            = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Bankroll IS NULL OR @Bankroll < 200
    BEGIN
        RAISERROR('Arbitrage: @Bankroll must be at least 200 COP.', 16, 1);
        RETURN;
    END;

    DECLARE @Banca DECIMAL(18, 0) = CAST(FLOOR(@Bankroll / 100.0) * 100 AS DECIMAL(18, 0));

    IF @Banca < 200
    BEGIN
        RAISERROR('Arbitrage: @Bankroll rounded to a 100 multiple must be at least 200 COP.', 16, 1);
        RETURN;
    END;

    DECLARE @Sports TABLE
    (
        Sport VARCHAR(64) NOT NULL PRIMARY KEY
    );

    IF @Sport IS NOT NULL AND LTRIM(RTRIM(@Sport)) <> ''
    BEGIN
        INSERT INTO @Sports (Sport)
        SELECT DISTINCT LOWER(LTRIM(RTRIM(value)))
        FROM STRING_SPLIT(@Sport, ',')
        WHERE LTRIM(RTRIM(value)) <> '';
    END;

    DECLARE @SportCount INT = (SELECT COUNT(*) FROM @Sports);

    ;WITH TwoWay AS
    (
        SELECT o.Id
        FROM dbo.ArbitrageOpportunities AS o
        INNER JOIN dbo.ArbitrageOpportunityLegs AS l ON l.ArbitrageId = o.Id
        WHERE (@SportCount = 0 OR EXISTS (SELECT 1 FROM @Sports AS s WHERE s.Sport = o.Sport))
          AND (@IsLive IS NULL OR o.IsLive = @IsLive)
          AND (@MinProfitPercent IS NULL OR ISNULL(o.ProfitPercent, 0) >= @MinProfitPercent)
        GROUP BY o.Id
        HAVING COUNT(*) = 2
    ),
    Ranked AS
    (
        SELECT
            o.Id,
            o.Sport,
            o.League,
            o.EventName,
            o.StartTime,
            o.IsLive,
            o.CreatedAtUtc,
            o.MarketType,
            o.Line,
            o.ProfitPercent,
            l.Sportsbook,
            l.Selection,
            l.OddsDecimal,
            l.StakePercent,
            rn = ROW_NUMBER() OVER (
                PARTITION BY o.Id
                ORDER BY l.StakePercent DESC, l.Sportsbook, l.Selection)
        FROM dbo.ArbitrageOpportunities AS o
        INNER JOIN TwoWay AS t ON t.Id = o.Id
        INNER JOIN dbo.ArbitrageOpportunityLegs AS l ON l.ArbitrageId = o.Id
        WHERE l.OddsDecimal > 1
    ),
    Pairs AS
    (
        SELECT
            a.Sport,
            a.League,
            a.EventName,
            a.StartTime,
            a.IsLive,
            a.CreatedAtUtc,
            a.MarketType,
            a.Line,
            CasaApuesta1 = a.Sportsbook,
            Equipo1      = a.Selection,
            Cuota1       = a.OddsDecimal,
            CasaApuesta2 = b.Sportsbook,
            Equipo2      = b.Selection,
            Cuota2       = b.OddsDecimal,
            Monto1 = CAST(ROUND(@Banca * ISNULL(a.StakePercent, 0) / 100.0 / 100.0, 0) * 100 AS DECIMAL(18, 0))
        FROM Ranked AS a
        INNER JOIN Ranked AS b
            ON b.Id = a.Id
           AND a.rn = 1
           AND b.rn = 2
    ),
    Stakes AS
    (
        SELECT
            Sport,
            League,
            EventName,
            StartTime,
            IsLive,
            CreatedAtUtc,
            MarketType,
            Line,
            CasaApuesta1,
            Equipo1,
            Cuota1,
            Monto1,
            CasaApuesta2,
            Equipo2,
            Cuota2,
            Monto2 = CAST(@Banca - Monto1 AS DECIMAL(18, 0)),
            Banca  = @Banca
        FROM Pairs
        WHERE Monto1 >= 100
          AND (@Banca - Monto1) >= 100
    )
    SELECT
        Deporte            = Sport,
        League,
        Partido            = EventName,
        EventStartTime     = StartTime,
        EnVivo             = IsLive,
        CreatedAtUtc,
        BetType            = CASE
                                WHEN Line IS NULL THEN MarketType
                                ELSE CONCAT(MarketType, ' ', CONVERT(VARCHAR(32), CAST(Line AS DECIMAL(18, 4))))
                             END,
        CasaApuesta1,
        Equipo1,
        Cuota1,
        Monto1,
        CasaApuesta2,
        Equipo2,
        Cuota2,
        Monto2,
        Banca,
        Instruccion        = CONCAT(
                                'En ', CasaApuesta1, ' apuesta ', CONVERT(VARCHAR(32), Monto1),
                                ' COP a ', Equipo1, ' (cuota ', CONVERT(VARCHAR(32), CAST(Cuota1 AS DECIMAL(18, 4))), '). ',
                                'En ', CasaApuesta2, ' apuesta ', CONVERT(VARCHAR(32), Monto2),
                                ' COP a ', Equipo2, ' (cuota ', CONVERT(VARCHAR(32), CAST(Cuota2 AS DECIMAL(18, 4))), ').'
                             ),
        GananciaEsperada   = CAST(
                                CASE
                                    WHEN Monto1 * Cuota1 < Monto2 * Cuota2 THEN Monto1 * Cuota1
                                    ELSE Monto2 * Cuota2
                                END - Banca
                             AS DECIMAL(18, 2)),
        TotalNetoGanado    = CAST(
                                CASE
                                    WHEN Monto1 * Cuota1 < Monto2 * Cuota2 THEN Monto1 * Cuota1
                                    ELSE Monto2 * Cuota2
                                END
                             AS DECIMAL(18, 2))
    FROM Stakes
    WHERE CASE
              WHEN Monto1 * Cuota1 < Monto2 * Cuota2 THEN Monto1 * Cuota1
              ELSE Monto2 * Cuota2
          END - Banca > 0
    ORDER BY GananciaEsperada DESC, EventStartTime;
END;
GO

EXEC dbo.spr_ArbitrageOpportunities
    @Bankroll         = 30000,
    @MinProfitPercent = 0;
```

## Arbitraje a 2 vías

Hay **cuatro procedimientos**. No se pueden mezclar: cada familia empareja las dos vías de forma distinta.

| Procedimiento | Familia | Cómo se cruzan las dos apuestas |
| --- | --- | --- |
| `spr_MarketOdds_Arbitrage` | Ganador directo (`moneyline`, `draw_no_bet` y variantes por periodo) | Local vs visitante. Sin línea. |
| `spr_MarketOdds_Arbitrage_Totals` | Totales (`total_*`, `team_total` y variantes) | Over vs Under **en la misma línea** (y el mismo equipo si es `team_total`). |
| `spr_MarketOdds_Arbitrage_Spreads` | Hándicap 2 vías (`point_spread`, `run_line`, `puck_line`, `asian_handicap`, `game_spread`, `*_handicap`) | Local **-X** vs visitante **+X** (líneas opuestas). |
| `spr_MarketOdds_Arbitrage_YesNo` | Evento sí/no (`both_teams_to_score`, `to_win_to_nil`, `to_score`, `go_the_distance`) | Yes vs No. Si aplica, el mismo equipo. |

Nunca 1X2 / `moneyline_3-way` / empate. En fútbol el `moneyline` (y cualquier `*moneyline*`) es tres caminos: si apuestas local en una casa y visitante en otra y empata, pierdes las dos. Por eso el SP de ganador los excluye; ahí entra `draw_no_bet`.

Los totales y hándicaps de cuarto de línea (`*.25` / `*.75`) se excluyen: no son dos resultados limpios (medio gana / medio pierde) y el stake del arb quedaría mal calculado.

`BetType` es el `MarketType`. `CasaApuesta1` + `Equipo1` + `Monto1` es la primera apuesta; `CasaApuesta2` + `Equipo2` + `Monto2` es la segunda. `Instruccion` junta las dos en una frase.

`@Bankroll` es el total en COP a repartir (por defecto 30000). `Monto1` + `Monto2` = banca. `GananciaEsperada` es la ganancia neta garantizada. `TotalNetoGanado` es lo que cobras (banca + ganancia).

### Ganador directo — `spr_MarketOdds_Arbitrage`

```sql
CREATE OR ALTER PROCEDURE dbo.spr_MarketOdds_Arbitrage
    @Sport           VARCHAR(64)    = NULL,
    @MinArbPercent   DECIMAL(18, 8) = 0,
    @IsLive          BIT            = 0,
    @IsMainLine      BIT            = 1,
    @Bankroll        DECIMAL(18, 2) = 30000
AS
BEGIN
    SET NOCOUNT ON;

    IF @Bankroll IS NULL OR @Bankroll <= 0
    BEGIN
        RAISERROR('OddsFeed: @Bankroll must be greater than zero.', 16, 1);
        RETURN;
    END;

    ;WITH Base AS
    (
        SELECT
            o.Id,
            o.Sport,
            o.League,
            o.HomeTeam,
            o.AwayTeam,
            o.EventStartTime,
            o.MarketType,
            o.Sportsbook,
            o.Selection,
            o.OddsDecimal,
            o.[Timestamp],
            EventKey = COALESCE(NULLIF(o.EventUuid, ''), o.EventId),
            Side     = LOWER(COALESCE(NULLIF(o.TeamSide, ''), NULLIF(o.SelectionType, ''), ''))
        FROM dbo.MarketOdds AS o
        WHERE o.IsActive = 1
          AND o.IsLive = @IsLive
          AND o.IsMainLine = @IsMainLine
          AND o.IsPlayerProp = 0
          AND o.OddsDecimal > 1
          AND (
                o.MarketType LIKE '%moneyline%'
                OR o.MarketType LIKE '%draw_no_bet%'
              )
          AND o.MarketType NOT LIKE '%3-way%'
          AND o.MarketType NOT LIKE '%3_way%'
          AND NOT (
                o.Sport IN ('soccer', 'football')
                AND o.MarketType LIKE '%moneyline%'
              )
          AND LOWER(COALESCE(NULLIF(o.TeamSide, ''), NULLIF(o.SelectionType, ''), '')) IN ('home', 'away', '1', '2', 'local', 'visitante')
          AND (@Sport IS NULL OR o.Sport = @Sport)
    ),
    Eligible AS
    (
        SELECT EventKey, MarketType
        FROM Base
        GROUP BY EventKey, MarketType
        HAVING COUNT(DISTINCT Side) = 2
    ),
    Ranked AS
    (
        SELECT
            b.*,
            rn = ROW_NUMBER() OVER (
                PARTITION BY b.EventKey, b.MarketType, b.Sportsbook, b.Side
                ORDER BY b.OddsDecimal DESC, b.[Timestamp] DESC)
        FROM Base AS b
        INNER JOIN Eligible AS e
            ON e.EventKey = b.EventKey
           AND e.MarketType = b.MarketType
    ),
    Legs AS
    (
        SELECT
            r.*,
            Equipo = CASE
                WHEN r.Side IN ('home', '1', 'local')
                    THEN COALESCE(NULLIF(r.HomeTeam, ''), NULLIF(r.Selection, ''), 'local')
                ELSE COALESCE(NULLIF(r.AwayTeam, ''), NULLIF(r.Selection, ''), 'visitante')
            END
        FROM Ranked AS r
        WHERE r.rn = 1
    ),
    Pairs AS
    (
        SELECT
            Soccer             = a.Sport,
            a.League,
            Partido            = CONCAT(COALESCE(a.HomeTeam, '?'), ' vs ', COALESCE(a.AwayTeam, '?')),
            a.EventStartTime,
            BetType            = a.MarketType,
            CasaApuesta1       = a.Sportsbook,
            Equipo1            = a.Equipo,
            Cuota1             = a.OddsDecimal,
            CasaApuesta2       = b.Sportsbook,
            Equipo2            = b.Equipo,
            Cuota2             = b.OddsDecimal,
            ImpliedSum         = (1.0 / a.OddsDecimal) + (1.0 / b.OddsDecimal)
        FROM Legs AS a
        INNER JOIN Legs AS b
            ON b.EventKey = a.EventKey
           AND b.MarketType = a.MarketType
           AND b.Side > a.Side
        WHERE a.Sportsbook <> b.Sportsbook
          AND LOWER(a.Selection) <> LOWER(b.Selection)
          AND ((1.0 / a.OddsDecimal) + (1.0 / b.OddsDecimal)) < 1.0
          AND ((1.0 - ((1.0 / a.OddsDecimal) + (1.0 / b.OddsDecimal))) * 100.0) >= @MinArbPercent
    ),
    Stakes AS
    (
        SELECT
            Soccer, League, Partido, EventStartTime, BetType,
            CasaApuesta1, Equipo1, Cuota1,
            Monto1 = CAST(ROUND(@Bankroll * (1.0 / Cuota1) / ImpliedSum, 0) AS DECIMAL(18, 0)),
            CasaApuesta2, Equipo2, Cuota2,
            Monto2 = CAST(@Bankroll - ROUND(@Bankroll * (1.0 / Cuota1) / ImpliedSum, 0) AS DECIMAL(18, 0)),
            Banca  = CAST(@Bankroll AS DECIMAL(18, 0))
        FROM Pairs
    )
    SELECT
        Soccer, League, Partido, EventStartTime, BetType,
        CasaApuesta1, Equipo1, Cuota1, Monto1,
        CasaApuesta2, Equipo2, Cuota2, Monto2, Banca,
        Instruccion        = CONCAT(
                                'En ', CasaApuesta1, ' apuesta ', CONVERT(VARCHAR(32), Monto1),
                                ' COP a ', Equipo1, ' (cuota ', CONVERT(VARCHAR(32), CAST(Cuota1 AS DECIMAL(18, 4))), '). ',
                                'En ', CasaApuesta2, ' apuesta ', CONVERT(VARCHAR(32), Monto2),
                                ' COP a ', Equipo2, ' (cuota ', CONVERT(VARCHAR(32), CAST(Cuota2 AS DECIMAL(18, 4))), ').'
                             ),
        GananciaEsperada   = CAST(
                                CASE WHEN Monto1 * Cuota1 < Monto2 * Cuota2 THEN Monto1 * Cuota1 ELSE Monto2 * Cuota2 END - Banca
                             AS DECIMAL(18, 2)),
        TotalNetoGanado    = CAST(
                                CASE WHEN Monto1 * Cuota1 < Monto2 * Cuota2 THEN Monto1 * Cuota1 ELSE Monto2 * Cuota2 END
                             AS DECIMAL(18, 2))
    FROM Stakes
    ORDER BY GananciaEsperada DESC, EventStartTime;
END;
GO
```

### Totales — `spr_MarketOdds_Arbitrage_Totals`

Over y Under deben compartir **la misma línea**. En `team_total` también el mismo equipo: Over del local no se cruza con Under del visitante.

```sql
CREATE OR ALTER PROCEDURE dbo.spr_MarketOdds_Arbitrage_Totals
    @Sport           VARCHAR(64)    = NULL,
    @MinArbPercent   DECIMAL(18, 8) = 0,
    @IsLive          BIT            = 0,
    @IsMainLine      BIT            = 1,
    @Bankroll        DECIMAL(18, 2) = 30000
AS
BEGIN
    SET NOCOUNT ON;

    IF @Bankroll IS NULL OR @Bankroll <= 0
    BEGIN
        RAISERROR('OddsFeed: @Bankroll must be greater than zero.', 16, 1);
        RETURN;
    END;

    ;WITH Base AS
    (
        SELECT
            o.Id,
            o.Sport,
            o.League,
            o.HomeTeam,
            o.AwayTeam,
            o.EventStartTime,
            o.MarketType,
            o.Sportsbook,
            o.Selection,
            o.OddsDecimal,
            o.Line,
            o.[Timestamp],
            EventKey = COALESCE(NULLIF(o.EventUuid, ''), o.EventId),
            Side     = CASE
                           WHEN LOWER(COALESCE(o.SelectionType, '')) IN ('over', 'under')
                               THEN LOWER(o.SelectionType)
                           WHEN LOWER(o.Selection) LIKE '%over%'  THEN 'over'
                           WHEN LOWER(o.Selection) LIKE '%under%' THEN 'under'
                           ELSE ''
                       END,
            TeamKey  = LOWER(COALESCE(NULLIF(o.TeamSide, ''), ''))
        FROM dbo.MarketOdds AS o
        WHERE o.IsActive = 1
          AND o.IsLive = @IsLive
          AND o.IsMainLine = @IsMainLine
          AND o.IsPlayerProp = 0
          AND o.OddsDecimal > 1
          AND o.Line IS NOT NULL
          AND ((ABS(o.Line) * 4.0) % 2.0 = 0)
          AND o.MarketType LIKE '%total%'
          AND o.MarketType NOT LIKE '%3-way%'
          AND o.MarketType NOT LIKE '%3_way%'
          AND (@Sport IS NULL OR o.Sport = @Sport)
    ),
    Filtered AS
    (
        SELECT *
        FROM Base
        WHERE Side IN ('over', 'under')
    ),
    Eligible AS
    (
        SELECT EventKey, MarketType, Line, TeamKey
        FROM Filtered
        GROUP BY EventKey, MarketType, Line, TeamKey
        HAVING COUNT(DISTINCT Side) = 2
    ),
    Ranked AS
    (
        SELECT
            b.*,
            rn = ROW_NUMBER() OVER (
                PARTITION BY b.EventKey, b.MarketType, b.Line, b.TeamKey, b.Sportsbook, b.Side
                ORDER BY b.OddsDecimal DESC, b.[Timestamp] DESC)
        FROM Filtered AS b
        INNER JOIN Eligible AS e
            ON e.EventKey = b.EventKey
           AND e.MarketType = b.MarketType
           AND e.Line = b.Line
           AND e.TeamKey = b.TeamKey
    ),
    Legs AS
    (
        SELECT
            r.*,
            Equipo = CONCAT(
                CASE WHEN r.Side = 'over' THEN 'Over' ELSE 'Under' END,
                CASE
                    WHEN r.Line > 0 THEN CONCAT(' +', CONVERT(VARCHAR(32), CAST(r.Line AS DECIMAL(18, 2))))
                    ELSE CONCAT(' ', CONVERT(VARCHAR(32), CAST(r.Line AS DECIMAL(18, 2))))
                END,
                CASE
                    WHEN r.TeamKey IN ('home', '1', 'local')
                        THEN CONCAT(' ', COALESCE(NULLIF(r.HomeTeam, ''), 'local'))
                    WHEN r.TeamKey IN ('away', '2', 'visitante')
                        THEN CONCAT(' ', COALESCE(NULLIF(r.AwayTeam, ''), 'visitante'))
                    ELSE ''
                END
            )
        FROM Ranked AS r
        WHERE r.rn = 1
    ),
    Pairs AS
    (
        SELECT
            Soccer             = a.Sport,
            a.League,
            Partido            = CONCAT(COALESCE(a.HomeTeam, '?'), ' vs ', COALESCE(a.AwayTeam, '?')),
            a.EventStartTime,
            BetType            = a.MarketType,
            CasaApuesta1       = a.Sportsbook,
            Equipo1            = a.Equipo,
            Cuota1             = a.OddsDecimal,
            CasaApuesta2       = b.Sportsbook,
            Equipo2            = b.Equipo,
            Cuota2             = b.OddsDecimal,
            ImpliedSum         = (1.0 / a.OddsDecimal) + (1.0 / b.OddsDecimal)
        FROM Legs AS a
        INNER JOIN Legs AS b
            ON b.EventKey = a.EventKey
           AND b.MarketType = a.MarketType
           AND b.Line = a.Line
           AND b.TeamKey = a.TeamKey
           AND b.Side > a.Side
        WHERE a.Sportsbook <> b.Sportsbook
          AND ((1.0 / a.OddsDecimal) + (1.0 / b.OddsDecimal)) < 1.0
          AND ((1.0 - ((1.0 / a.OddsDecimal) + (1.0 / b.OddsDecimal))) * 100.0) >= @MinArbPercent
    ),
    Stakes AS
    (
        SELECT
            Soccer, League, Partido, EventStartTime, BetType,
            CasaApuesta1, Equipo1, Cuota1,
            Monto1 = CAST(ROUND(@Bankroll * (1.0 / Cuota1) / ImpliedSum, 0) AS DECIMAL(18, 0)),
            CasaApuesta2, Equipo2, Cuota2,
            Monto2 = CAST(@Bankroll - ROUND(@Bankroll * (1.0 / Cuota1) / ImpliedSum, 0) AS DECIMAL(18, 0)),
            Banca  = CAST(@Bankroll AS DECIMAL(18, 0))
        FROM Pairs
    )
    SELECT
        Soccer, League, Partido, EventStartTime, BetType,
        CasaApuesta1, Equipo1, Cuota1, Monto1,
        CasaApuesta2, Equipo2, Cuota2, Monto2, Banca,
        Instruccion        = CONCAT(
                                'En ', CasaApuesta1, ' apuesta ', CONVERT(VARCHAR(32), Monto1),
                                ' COP a ', Equipo1, ' (cuota ', CONVERT(VARCHAR(32), CAST(Cuota1 AS DECIMAL(18, 4))), '). ',
                                'En ', CasaApuesta2, ' apuesta ', CONVERT(VARCHAR(32), Monto2),
                                ' COP a ', Equipo2, ' (cuota ', CONVERT(VARCHAR(32), CAST(Cuota2 AS DECIMAL(18, 4))), ').'
                             ),
        GananciaEsperada   = CAST(
                                CASE WHEN Monto1 * Cuota1 < Monto2 * Cuota2 THEN Monto1 * Cuota1 ELSE Monto2 * Cuota2 END - Banca
                             AS DECIMAL(18, 2)),
        TotalNetoGanado    = CAST(
                                CASE WHEN Monto1 * Cuota1 < Monto2 * Cuota2 THEN Monto1 * Cuota1 ELSE Monto2 * Cuota2 END
                             AS DECIMAL(18, 2))
    FROM Stakes
    ORDER BY GananciaEsperada DESC, EventStartTime;
END;
GO
```

### Hándicap 2 vías — `spr_MarketOdds_Arbitrage_Spreads`

No se cruza la misma línea. El local con **-3.5** se empareja con el visitante con **+3.5**.

```sql
CREATE OR ALTER PROCEDURE dbo.spr_MarketOdds_Arbitrage_Spreads
    @Sport           VARCHAR(64)    = NULL,
    @MinArbPercent   DECIMAL(18, 8) = 0,
    @IsLive          BIT            = 0,
    @IsMainLine      BIT            = 1,
    @Bankroll        DECIMAL(18, 2) = 30000
AS
BEGIN
    SET NOCOUNT ON;

    IF @Bankroll IS NULL OR @Bankroll <= 0
    BEGIN
        RAISERROR('OddsFeed: @Bankroll must be greater than zero.', 16, 1);
        RETURN;
    END;

    ;WITH Base AS
    (
        SELECT
            o.Id,
            o.Sport,
            o.League,
            o.HomeTeam,
            o.AwayTeam,
            o.EventStartTime,
            o.MarketType,
            o.Sportsbook,
            o.Selection,
            o.OddsDecimal,
            o.Line,
            o.[Timestamp],
            EventKey = COALESCE(NULLIF(o.EventUuid, ''), o.EventId),
            Side     = LOWER(COALESCE(NULLIF(o.TeamSide, ''), NULLIF(o.SelectionType, ''), ''))
        FROM dbo.MarketOdds AS o
        WHERE o.IsActive = 1
          AND o.IsLive = @IsLive
          AND o.IsMainLine = @IsMainLine
          AND o.IsPlayerProp = 0
          AND o.OddsDecimal > 1
          AND o.Line IS NOT NULL
          AND ((ABS(o.Line) * 4.0) % 2.0 = 0)
          AND (
                o.MarketType LIKE '%spread%'
                OR o.MarketType LIKE '%run_line%'
                OR o.MarketType LIKE '%puck_line%'
                OR o.MarketType LIKE '%handicap%'
              )
          AND o.MarketType NOT LIKE '%3-way%'
          AND o.MarketType NOT LIKE '%3_way%'
          AND LOWER(COALESCE(NULLIF(o.TeamSide, ''), NULLIF(o.SelectionType, ''), '')) IN ('home', 'away', '1', '2', 'local', 'visitante')
          AND (@Sport IS NULL OR o.Sport = @Sport)
    ),
    Eligible AS
    (
        SELECT EventKey, MarketType, LineAbs = ABS(Line)
        FROM Base
        GROUP BY EventKey, MarketType, ABS(Line)
        HAVING COUNT(DISTINCT Side) = 2
    ),
    Ranked AS
    (
        SELECT
            b.*,
            rn = ROW_NUMBER() OVER (
                PARTITION BY b.EventKey, b.MarketType, b.Line, b.Sportsbook, b.Side
                ORDER BY b.OddsDecimal DESC, b.[Timestamp] DESC)
        FROM Base AS b
        INNER JOIN Eligible AS e
            ON e.EventKey = b.EventKey
           AND e.MarketType = b.MarketType
           AND e.LineAbs = ABS(b.Line)
    ),
    Legs AS
    (
        SELECT
            r.*,
            Equipo = CONCAT(
                CASE
                    WHEN r.Side IN ('home', '1', 'local')
                        THEN COALESCE(NULLIF(r.HomeTeam, ''), NULLIF(r.Selection, ''), 'local')
                    ELSE COALESCE(NULLIF(r.AwayTeam, ''), NULLIF(r.Selection, ''), 'visitante')
                END,
                CASE
                    WHEN r.Line > 0 THEN CONCAT(' +', CONVERT(VARCHAR(32), CAST(r.Line AS DECIMAL(18, 2))))
                    ELSE CONCAT(' ', CONVERT(VARCHAR(32), CAST(r.Line AS DECIMAL(18, 2))))
                END
            )
        FROM Ranked AS r
        WHERE r.rn = 1
    ),
    Pairs AS
    (
        SELECT
            Soccer             = a.Sport,
            a.League,
            Partido            = CONCAT(COALESCE(a.HomeTeam, '?'), ' vs ', COALESCE(a.AwayTeam, '?')),
            a.EventStartTime,
            BetType            = a.MarketType,
            CasaApuesta1       = a.Sportsbook,
            Equipo1            = a.Equipo,
            Cuota1             = a.OddsDecimal,
            CasaApuesta2       = b.Sportsbook,
            Equipo2            = b.Equipo,
            Cuota2             = b.OddsDecimal,
            ImpliedSum         = (1.0 / a.OddsDecimal) + (1.0 / b.OddsDecimal)
        FROM Legs AS a
        INNER JOIN Legs AS b
            ON b.EventKey = a.EventKey
           AND b.MarketType = a.MarketType
           AND b.Line = -a.Line
           AND b.Side > a.Side
        WHERE a.Sportsbook <> b.Sportsbook
          AND LOWER(a.Selection) <> LOWER(b.Selection)
          AND ((1.0 / a.OddsDecimal) + (1.0 / b.OddsDecimal)) < 1.0
          AND ((1.0 - ((1.0 / a.OddsDecimal) + (1.0 / b.OddsDecimal))) * 100.0) >= @MinArbPercent
    ),
    Stakes AS
    (
        SELECT
            Soccer, League, Partido, EventStartTime, BetType,
            CasaApuesta1, Equipo1, Cuota1,
            Monto1 = CAST(ROUND(@Bankroll * (1.0 / Cuota1) / ImpliedSum, 0) AS DECIMAL(18, 0)),
            CasaApuesta2, Equipo2, Cuota2,
            Monto2 = CAST(@Bankroll - ROUND(@Bankroll * (1.0 / Cuota1) / ImpliedSum, 0) AS DECIMAL(18, 0)),
            Banca  = CAST(@Bankroll AS DECIMAL(18, 0))
        FROM Pairs
    )
    SELECT
        Soccer, League, Partido, EventStartTime, BetType,
        CasaApuesta1, Equipo1, Cuota1, Monto1,
        CasaApuesta2, Equipo2, Cuota2, Monto2, Banca,
        Instruccion        = CONCAT(
                                'En ', CasaApuesta1, ' apuesta ', CONVERT(VARCHAR(32), Monto1),
                                ' COP a ', Equipo1, ' (cuota ', CONVERT(VARCHAR(32), CAST(Cuota1 AS DECIMAL(18, 4))), '). ',
                                'En ', CasaApuesta2, ' apuesta ', CONVERT(VARCHAR(32), Monto2),
                                ' COP a ', Equipo2, ' (cuota ', CONVERT(VARCHAR(32), CAST(Cuota2 AS DECIMAL(18, 4))), ').'
                             ),
        GananciaEsperada   = CAST(
                                CASE WHEN Monto1 * Cuota1 < Monto2 * Cuota2 THEN Monto1 * Cuota1 ELSE Monto2 * Cuota2 END - Banca
                             AS DECIMAL(18, 2)),
        TotalNetoGanado    = CAST(
                                CASE WHEN Monto1 * Cuota1 < Monto2 * Cuota2 THEN Monto1 * Cuota1 ELSE Monto2 * Cuota2 END
                             AS DECIMAL(18, 2))
    FROM Stakes
    ORDER BY GananciaEsperada DESC, EventStartTime;
END;
GO
```

### Evento sí / no — `spr_MarketOdds_Arbitrage_YesNo`

Yes se cruza con No. En `to_score` se agrupa también por equipo para no mezclar el local con el visitante.

```sql
CREATE OR ALTER PROCEDURE dbo.spr_MarketOdds_Arbitrage_YesNo
    @Sport           VARCHAR(64)    = NULL,
    @MinArbPercent   DECIMAL(18, 8) = 0,
    @IsLive          BIT            = 0,
    @IsMainLine      BIT            = 1,
    @Bankroll        DECIMAL(18, 2) = 30000
AS
BEGIN
    SET NOCOUNT ON;

    IF @Bankroll IS NULL OR @Bankroll <= 0
    BEGIN
        RAISERROR('OddsFeed: @Bankroll must be greater than zero.', 16, 1);
        RETURN;
    END;

    ;WITH Base AS
    (
        SELECT
            o.Id,
            o.Sport,
            o.League,
            o.HomeTeam,
            o.AwayTeam,
            o.EventStartTime,
            o.MarketType,
            o.Sportsbook,
            o.Selection,
            o.OddsDecimal,
            o.Line,
            o.[Timestamp],
            EventKey = COALESCE(NULLIF(o.EventUuid, ''), o.EventId),
            Side     = CASE
                           WHEN LOWER(COALESCE(o.SelectionType, '')) IN ('yes', 'no')
                               THEN LOWER(o.SelectionType)
                           WHEN LOWER(o.Selection) IN ('yes', 'sí', 'si') THEN 'yes'
                           WHEN LOWER(o.Selection) IN ('no') THEN 'no'
                           ELSE ''
                       END,
            TeamKey  = LOWER(COALESCE(NULLIF(o.TeamSide, ''), ''))
        FROM dbo.MarketOdds AS o
        WHERE o.IsActive = 1
          AND o.IsLive = @IsLive
          AND o.IsMainLine = @IsMainLine
          AND o.IsPlayerProp = 0
          AND o.OddsDecimal > 1
          AND (
                o.MarketType LIKE '%both_teams_to_score%'
                OR o.MarketType LIKE '%to_win_to_nil%'
                OR o.MarketType LIKE '%go_the_distance%'
                OR (
                    (o.MarketType = 'to_score' OR o.MarketType LIKE '%[_]to_score' OR o.MarketType LIKE 'to_score[_]%')
                    AND o.MarketType NOT LIKE '%first_to_score%'
                    AND o.MarketType NOT LIKE '%next_to_score%'
                    AND o.MarketType NOT LIKE '%last_to_score%'
                )
              )
          AND o.MarketType NOT LIKE '%3-way%'
          AND o.MarketType NOT LIKE '%3_way%'
          AND (@Sport IS NULL OR o.Sport = @Sport)
    ),
    Filtered AS
    (
        SELECT *
        FROM Base
        WHERE Side IN ('yes', 'no')
    ),
    Eligible AS
    (
        SELECT EventKey, MarketType, Line, TeamKey
        FROM Filtered
        GROUP BY EventKey, MarketType, Line, TeamKey
        HAVING COUNT(DISTINCT Side) = 2
    ),
    Ranked AS
    (
        SELECT
            b.*,
            rn = ROW_NUMBER() OVER (
                PARTITION BY b.EventKey, b.MarketType, b.Line, b.TeamKey, b.Sportsbook, b.Side
                ORDER BY b.OddsDecimal DESC, b.[Timestamp] DESC)
        FROM Filtered AS b
        INNER JOIN Eligible AS e
            ON e.EventKey = b.EventKey
           AND e.MarketType = b.MarketType
           AND ((e.Line = b.Line) OR (e.Line IS NULL AND b.Line IS NULL))
           AND e.TeamKey = b.TeamKey
    ),
    Legs AS
    (
        SELECT
            r.*,
            Equipo = CONCAT(
                CASE WHEN r.Side = 'yes' THEN 'Sí' ELSE 'No' END,
                CASE
                    WHEN r.TeamKey IN ('home', '1', 'local')
                        THEN CONCAT(' ', COALESCE(NULLIF(r.HomeTeam, ''), 'local'))
                    WHEN r.TeamKey IN ('away', '2', 'visitante')
                        THEN CONCAT(' ', COALESCE(NULLIF(r.AwayTeam, ''), 'visitante'))
                    WHEN NULLIF(r.Selection, '') IS NOT NULL
                         AND LOWER(r.Selection) NOT IN ('yes', 'no', 'sí', 'si')
                        THEN CONCAT(' ', r.Selection)
                    ELSE ''
                END
            )
        FROM Ranked AS r
        WHERE r.rn = 1
    ),
    Pairs AS
    (
        SELECT
            Soccer             = a.Sport,
            a.League,
            Partido            = CONCAT(COALESCE(a.HomeTeam, '?'), ' vs ', COALESCE(a.AwayTeam, '?')),
            a.EventStartTime,
            BetType            = a.MarketType,
            CasaApuesta1       = a.Sportsbook,
            Equipo1            = a.Equipo,
            Cuota1             = a.OddsDecimal,
            CasaApuesta2       = b.Sportsbook,
            Equipo2            = b.Equipo,
            Cuota2             = b.OddsDecimal,
            ImpliedSum         = (1.0 / a.OddsDecimal) + (1.0 / b.OddsDecimal)
        FROM Legs AS a
        INNER JOIN Legs AS b
            ON b.EventKey = a.EventKey
           AND b.MarketType = a.MarketType
           AND ((b.Line = a.Line) OR (a.Line IS NULL AND b.Line IS NULL))
           AND b.TeamKey = a.TeamKey
           AND b.Side > a.Side
        WHERE a.Sportsbook <> b.Sportsbook
          AND ((1.0 / a.OddsDecimal) + (1.0 / b.OddsDecimal)) < 1.0
          AND ((1.0 - ((1.0 / a.OddsDecimal) + (1.0 / b.OddsDecimal))) * 100.0) >= @MinArbPercent
    ),
    Stakes AS
    (
        SELECT
            Soccer, League, Partido, EventStartTime, BetType,
            CasaApuesta1, Equipo1, Cuota1,
            Monto1 = CAST(ROUND(@Bankroll * (1.0 / Cuota1) / ImpliedSum, 0) AS DECIMAL(18, 0)),
            CasaApuesta2, Equipo2, Cuota2,
            Monto2 = CAST(@Bankroll - ROUND(@Bankroll * (1.0 / Cuota1) / ImpliedSum, 0) AS DECIMAL(18, 0)),
            Banca  = CAST(@Bankroll AS DECIMAL(18, 0))
        FROM Pairs
    )
    SELECT
        Soccer, League, Partido, EventStartTime, BetType,
        CasaApuesta1, Equipo1, Cuota1, Monto1,
        CasaApuesta2, Equipo2, Cuota2, Monto2, Banca,
        Instruccion        = CONCAT(
                                'En ', CasaApuesta1, ' apuesta ', CONVERT(VARCHAR(32), Monto1),
                                ' COP a ', Equipo1, ' (cuota ', CONVERT(VARCHAR(32), CAST(Cuota1 AS DECIMAL(18, 4))), '). ',
                                'En ', CasaApuesta2, ' apuesta ', CONVERT(VARCHAR(32), Monto2),
                                ' COP a ', Equipo2, ' (cuota ', CONVERT(VARCHAR(32), CAST(Cuota2 AS DECIMAL(18, 4))), ').'
                             ),
        GananciaEsperada   = CAST(
                                CASE WHEN Monto1 * Cuota1 < Monto2 * Cuota2 THEN Monto1 * Cuota1 ELSE Monto2 * Cuota2 END - Banca
                             AS DECIMAL(18, 2)),
        TotalNetoGanado    = CAST(
                                CASE WHEN Monto1 * Cuota1 < Monto2 * Cuota2 THEN Monto1 * Cuota1 ELSE Monto2 * Cuota2 END
                             AS DECIMAL(18, 2))
    FROM Stakes
    ORDER BY GananciaEsperada DESC, EventStartTime;
END;
GO
```

```sql
EXEC dbo.spr_MarketOdds_Arbitrage
    @MinArbPercent = 0.000001,
    @Bankroll = 30000;

EXEC dbo.spr_MarketOdds_Arbitrage_Totals
    @MinArbPercent = 0.000001,
    @Bankroll = 30000;

EXEC dbo.spr_MarketOdds_Arbitrage_Spreads
    @MinArbPercent = 0.000001,
    @Bankroll = 30000;

EXEC dbo.spr_MarketOdds_Arbitrage_YesNo
    @MinArbPercent = 0.000001,
    @Bankroll = 30000;
```

