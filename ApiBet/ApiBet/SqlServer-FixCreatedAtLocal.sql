USE [Deportes];
GO

/* CreatedAtUtc: SQL Server ya está en hora local (Colombia).
   Los SP y defaults seguían con SYSUTCDATETIME() (+5 h). Pasar a SYSDATETIME(). */

DECLARE @ObjectId INT;
DECLARE @Sql NVARCHAR(MAX);

DECLARE proc_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT object_id, definition
    FROM sys.sql_modules
    WHERE definition LIKE N'%SYSUTCDATETIME()%';

OPEN proc_cursor;
FETCH NEXT FROM proc_cursor INTO @ObjectId, @Sql;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @Sql = REPLACE(@Sql, N'SYSUTCDATETIME()', N'SYSDATETIME()');

    IF @Sql NOT LIKE N'%CREATE OR ALTER%'
    BEGIN
        SET @Sql = STUFF(@Sql, CHARINDEX(N'CREATE', @Sql), 6, N'CREATE OR ALTER');
    END;

    EXEC sys.sp_executesql @Sql;
    FETCH NEXT FROM proc_cursor INTO @ObjectId, @Sql;
END;

CLOSE proc_cursor;
DEALLOCATE proc_cursor;
GO

IF OBJECT_ID(N'dbo.BestOdds', N'U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'dbo.DF_BestOdds_CreatedAtUtc', N'D') IS NOT NULL
        ALTER TABLE dbo.BestOdds DROP CONSTRAINT DF_BestOdds_CreatedAtUtc;

    ALTER TABLE dbo.BestOdds
        ADD CONSTRAINT DF_BestOdds_CreatedAtUtc DEFAULT (SYSDATETIME()) FOR CreatedAtUtc;
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

IF OBJECT_ID(N'dbo.MarketOddsDelta', N'U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'dbo.DF_MarketOddsDelta_CreatedAtUtc', N'D') IS NOT NULL
        ALTER TABLE dbo.MarketOddsDelta DROP CONSTRAINT DF_MarketOddsDelta_CreatedAtUtc;

    ALTER TABLE dbo.MarketOddsDelta
        ADD CONSTRAINT DF_MarketOddsDelta_CreatedAtUtc DEFAULT (SYSDATETIME()) FOR CreatedAtUtc;
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

SELECT OBJECT_NAME(object_id) AS ProcName,
       CASE WHEN definition LIKE N'%SYSUTCDATETIME%' THEN N'SYSUTCDATETIME'
            WHEN definition LIKE N'%SYSDATETIME%' THEN N'SYSDATETIME'
            ELSE N'NONE' END AS ClockFn
FROM sys.sql_modules
WHERE OBJECT_NAME(object_id) IN (
    N'spr_MarketOddsDelta_DeltaLoad',
    N'spr_MarketOdds_BulkLoad',
    N'spr_BestOdds_BulkLoad',
    N'spr_ArbitrageOpportunities_BulkLoad');

SELECT name, definition
FROM sys.default_constraints
WHERE parent_object_id IN (
    OBJECT_ID(N'dbo.MarketOddsDelta'),
    OBJECT_ID(N'dbo.MarketOdds'),
    OBJECT_ID(N'dbo.BestOdds'),
    OBJECT_ID(N'dbo.ArbitrageOpportunities'))
  AND name LIKE N'DF_%CreatedAtUtc';
GO
