using System.Data;
using System.Data.Common;
using System.Text.Json;
using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Persistencia SQL Server de las cuotas de mercado (GET /odds y GET /odds/delta).
    /// </summary>
    internal sealed class OddsRepository : IOddsRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Inicializa el repositorio con el contexto de SQL Server.
        /// </summary>
        public OddsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task ClearAllAsync(CancellationToken cancellationToken)
        {
            await _dbContext.MarketOdds.ExecuteDeleteAsync(cancellationToken);
            _dbContext.ChangeTracker.Clear();
        }

        /// <inheritdoc />
        public async Task ClearDeltaAsync(CancellationToken cancellationToken)
        {
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM dbo.MarketOddsDelta", cancellationToken);
            _dbContext.ChangeTracker.Clear();
        }

        /// <inheritdoc />
        public async Task<int> SavePageAsync(
            IReadOnlyList<MarketOdd> odds,
            DateTime? sourceUpdatedAt,
            CancellationToken cancellationToken)
        {
            if (odds.Count == 0)
            {
                return 0;
            }

            var payload = odds.Select(MapBulkOdd).ToArray();
            var json = JsonSerializer.Serialize(payload, BulkJson.Options);

            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "dbo.spr_MarketOdds_BulkLoad";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 180;
            AddParameter(command, "@Json", json);

            var oddCount = 0;
            await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    oddCount = Convert.ToInt32(reader.GetValue(0));
                }
            }

            await CatalogSyncStateStore.UpsertAsync(
                _dbContext,
                CatalogSyncKeys.MarketOdds,
                sourceUpdatedAt,
                DateTime.Now,
                cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            _dbContext.ChangeTracker.Clear();

            return oddCount;
        }

        /// <inheritdoc />
        public async Task<MarketOddsDeltaPersistResult> SaveDeltaAsync(
            IReadOnlyList<MarketOdd> odds,
            IReadOnlyList<RemovedOdd> removed,
            DateTime? sourceUpdatedAt,
            CancellationToken cancellationToken)
        {
            if (odds.Count == 0 && removed.Count == 0)
            {
                return new MarketOddsDeltaPersistResult(0, 0);
            }

            var payload = new DeltaBulkPayload
            {
                Data = odds
                    .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                    .Select(MapBulkOdd)
                    .ToArray(),
                Removed = removed
                    .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                    .Select(item => new DeltaRemovedPayload { Id = item.Id })
                    .ToArray()
            };

            var json = JsonSerializer.Serialize(payload, BulkJson.Options);

            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "dbo.spr_MarketOddsDelta_DeltaLoad";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 180;
            AddParameter(command, "@Json", json);

            var upsertedCount = 0;
            var deletedCount = 0;
            await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    upsertedCount = Convert.ToInt32(reader.GetValue(0));
                    deletedCount = Convert.ToInt32(reader.GetValue(1));
                }
            }

            await CatalogSyncStateStore.UpsertAsync(
                _dbContext,
                CatalogSyncKeys.MarketOddsDelta,
                sourceUpdatedAt,
                DateTime.Now,
                cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            _dbContext.ChangeTracker.Clear();

            return new MarketOddsDeltaPersistResult(upsertedCount, deletedCount);
        }

        /// <summary>
        /// Agrega un parámetro al comando ADO.NET.
        /// </summary>
        private static void AddParameter(DbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            if (value is string)
            {
                parameter.DbType = DbType.String;
                parameter.Size = -1;
            }

            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Proyecta la entidad de dominio al JSON esperado por spr_MarketOdds_BulkLoad.
        /// </summary>
        private static BulkMarketOddPayload MapBulkOdd(MarketOdd odd)
        {
            return new BulkMarketOddPayload
            {
                Id = odd.Id,
                Sportsbook = odd.Sportsbook,
                EventId = odd.EventId,
                EventUuid = odd.EventUuid,
                Sport = odd.Sport,
                League = odd.League,
                HomeTeam = odd.HomeTeam,
                AwayTeam = odd.AwayTeam,
                MarketType = odd.MarketType,
                Selection = odd.Selection,
                SelectionType = odd.SelectionType,
                TeamSide = odd.TeamSide,
                OddsAmerican = odd.OddsAmerican,
                OddsDecimal = odd.OddsDecimal,
                OddsProbability = odd.OddsProbability,
                Line = odd.Line,
                EventStartTime = odd.EventStartTime,
                IsLive = odd.IsLive,
                IsActive = odd.IsActive,
                Timestamp = odd.Timestamp,
                DeepLink = odd.DeepLink,
                MarketId = odd.MarketId,
                SelectionId = odd.SelectionId,
                ExternalMarketId = odd.ExternalMarketId,
                IsAlternateLine = odd.IsAlternateLine,
                IsMainLine = odd.IsMainLine,
                IsStalePregamePrice = odd.IsStalePregamePrice,
                IsPlayerProp = odd.IsPlayerProp,
                IsImpossibleScoreline = odd.IsImpossibleScoreline,
                SeasonType = odd.SeasonType,
                HomeId = odd.HomeId,
                HomeNumericalId = odd.HomeNumericalId,
                HomeName = odd.HomeName,
                HomeLogo = odd.HomeLogo,
                HomeAbbreviation = odd.HomeAbbreviation,
                HomeCity = odd.HomeCity,
                HomeMascot = odd.HomeMascot,
                HomeConference = odd.HomeConference,
                HomeDivision = odd.HomeDivision,
                AwayId = odd.AwayId,
                AwayNumericalId = odd.AwayNumericalId,
                AwayName = odd.AwayName,
                AwayLogo = odd.AwayLogo,
                AwayAbbreviation = odd.AwayAbbreviation,
                AwayCity = odd.AwayCity,
                AwayMascot = odd.AwayMascot,
                AwayConference = odd.AwayConference,
                AwayDivision = odd.AwayDivision,
                SportRefId = odd.SportRefId,
                SportRefName = odd.SportRefName,
                SportRefNumericalId = odd.SportRefNumericalId,
                LeagueRefId = odd.LeagueRefId,
                LeagueRefLabel = odd.LeagueRefLabel,
                LeagueRefNumericalId = odd.LeagueRefNumericalId,
                MarketRefId = odd.MarketRefId,
                MarketRefLabel = odd.MarketRefLabel,
                MarketRefNumericalId = odd.MarketRefNumericalId,
                SportsbookRefId = odd.SportsbookRefId,
                SportsbookRefName = odd.SportsbookRefName,
                SportsbookRefNumericalId = odd.SportsbookRefNumericalId,
                CompetitionRefId = odd.CompetitionRefId,
                CompetitionRefLabel = odd.CompetitionRefLabel,
                CompetitionRefNumericalId = odd.CompetitionRefNumericalId,
                ExternalEventId = odd.ExternalEventId
            };
        }

        /// <summary>
        /// Contrato JSON de una fila de MarketOdds para la carga masiva.
        /// </summary>
        private sealed class BulkMarketOddPayload
        {
            public required string Id { get; init; }
            public required string Sportsbook { get; init; }
            public required string EventId { get; init; }
            public string? EventUuid { get; init; }
            public required string Sport { get; init; }
            public required string League { get; init; }
            public string? HomeTeam { get; init; }
            public string? AwayTeam { get; init; }
            public required string MarketType { get; init; }
            public required string Selection { get; init; }
            public string? SelectionType { get; init; }
            public string? TeamSide { get; init; }
            public decimal? OddsAmerican { get; init; }
            public decimal? OddsDecimal { get; init; }
            public decimal? OddsProbability { get; init; }
            public decimal? Line { get; init; }
            public DateTime? EventStartTime { get; init; }
            public bool IsLive { get; init; }
            public bool IsActive { get; init; }
            public DateTime? Timestamp { get; init; }
            public string? DeepLink { get; init; }
            public string? MarketId { get; init; }
            public string? SelectionId { get; init; }
            public string? ExternalMarketId { get; init; }
            public bool IsAlternateLine { get; init; }
            public bool IsMainLine { get; init; }
            public bool IsStalePregamePrice { get; init; }
            public bool IsPlayerProp { get; init; }
            public bool IsImpossibleScoreline { get; init; }
            public string? SeasonType { get; init; }
            public string? HomeId { get; init; }
            public int? HomeNumericalId { get; init; }
            public string? HomeName { get; init; }
            public string? HomeLogo { get; init; }
            public string? HomeAbbreviation { get; init; }
            public string? HomeCity { get; init; }
            public string? HomeMascot { get; init; }
            public string? HomeConference { get; init; }
            public string? HomeDivision { get; init; }
            public string? AwayId { get; init; }
            public int? AwayNumericalId { get; init; }
            public string? AwayName { get; init; }
            public string? AwayLogo { get; init; }
            public string? AwayAbbreviation { get; init; }
            public string? AwayCity { get; init; }
            public string? AwayMascot { get; init; }
            public string? AwayConference { get; init; }
            public string? AwayDivision { get; init; }
            public string? SportRefId { get; init; }
            public string? SportRefName { get; init; }
            public int? SportRefNumericalId { get; init; }
            public string? LeagueRefId { get; init; }
            public string? LeagueRefLabel { get; init; }
            public int? LeagueRefNumericalId { get; init; }
            public string? MarketRefId { get; init; }
            public string? MarketRefLabel { get; init; }
            public int? MarketRefNumericalId { get; init; }
            public string? SportsbookRefId { get; init; }
            public string? SportsbookRefName { get; init; }
            public int? SportsbookRefNumericalId { get; init; }
            public string? CompetitionRefId { get; init; }
            public string? CompetitionRefLabel { get; init; }
            public int? CompetitionRefNumericalId { get; init; }
            public string? ExternalEventId { get; init; }
        }

        /// <summary>
        /// Contrato JSON enviado a spr_MarketOddsDelta_DeltaLoad.
        /// </summary>
        private sealed class DeltaBulkPayload
        {
            public required IReadOnlyList<BulkMarketOddPayload> Data { get; init; }

            public required IReadOnlyList<DeltaRemovedPayload> Removed { get; init; }
        }

        /// <summary>
        /// Identificador retirado dentro del JSON de carga incremental.
        /// </summary>
        private sealed class DeltaRemovedPayload
        {
            public required string Id { get; init; }
        }
    }
}
