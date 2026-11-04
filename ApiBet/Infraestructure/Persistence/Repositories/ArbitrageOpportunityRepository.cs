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
    /// Persistencia SQL Server de las oportunidades de arbitraje SharpAPI.
    /// </summary>
    internal sealed class ArbitrageOpportunityRepository : IArbitrageOpportunityRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Inicializa el repositorio con el contexto de SQL Server.
        /// </summary>
        public ArbitrageOpportunityRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<int> ClearExpiredAsync(int retentionMinutes, CancellationToken cancellationToken)
        {
            var minutes = Math.Max(retentionMinutes, 0);
            var deleted = await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ArbitrageOpportunities WHERE CreatedAtUtc <= DATEADD(MINUTE, -{minutes}, SYSDATETIME())",
                cancellationToken);
            _dbContext.ChangeTracker.Clear();
            return deleted;
        }

        /// <inheritdoc />
        public async Task<(int OpportunityCount, int LegCount)> SavePageAsync(
            IReadOnlyList<ArbitrageOpportunity> opportunities,
            DateTime? sourceUpdatedAt,
            string? sportsbooks,
            CancellationToken cancellationToken)
        {
            if (opportunities.Count == 0)
            {
                return (0, 0);
            }

            var payload = opportunities.Select(MapBulkOpportunity).ToArray();
            var json = JsonSerializer.Serialize(payload, BulkJson.Options);

            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "dbo.spr_ArbitrageOpportunities_BulkLoad";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 180;
            AddParameter(command, "@Json", json);
            AddParameter(
                command,
                "@Sportsbooks",
                string.IsNullOrWhiteSpace(sportsbooks) ? DBNull.Value : sportsbooks.Trim());

            var opportunityCount = 0;
            var legCount = 0;
            await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    opportunityCount = Convert.ToInt32(reader.GetValue(0));
                    legCount = Convert.ToInt32(reader.GetValue(1));
                }
            }

            await CatalogSyncStateStore.UpsertAsync(
                _dbContext,
                CatalogSyncKeys.ArbitrageOpportunities,
                sourceUpdatedAt,
                DateTime.Now,
                cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            _dbContext.ChangeTracker.Clear();

            return (opportunityCount, legCount);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ArbitrageBetRow>> GetBetsAsync(
            decimal bankroll,
            string? sport,
            decimal minProfitPercent,
            bool? isLive,
            CancellationToken cancellationToken)
        {
            var sportFilter = string.IsNullOrWhiteSpace(sport)
                ? null
                : sport.Trim().ToLowerInvariant().Replace(' ', '_');

            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "dbo.spr_ArbitrageOpportunities";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 180;
            AddParameter(command, "@Bankroll", bankroll);
            AddParameter(command, "@Sport", sportFilter is null ? DBNull.Value : sportFilter);
            AddParameter(command, "@MinProfitPercent", minProfitPercent);
            AddParameter(command, "@IsLive", isLive.HasValue ? isLive.Value : DBNull.Value);

            var rows = new List<ArbitrageBetRow>();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                rows.Add(MapBetRow(reader));
            }

            return rows;
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
        /// Lee una boleta del result set de dbo.spr_ArbitrageOpportunities.
        /// </summary>
        private static ArbitrageBetRow MapBetRow(DbDataReader reader)
        {
            return new ArbitrageBetRow(
                ReadString(reader, "Deporte"),
                ReadString(reader, "League"),
                ReadString(reader, "Partido"),
                ReadDateTime(reader, "EventStartTime"),
                ReadBoolean(reader, "EnVivo"),
                ReadDateTime(reader, "CreatedAtUtc") ?? default,
                ReadString(reader, "BetType"),
                ReadString(reader, "CasaApuesta1"),
                ReadString(reader, "Equipo1"),
                ReadDecimal(reader, "Cuota1"),
                ReadDecimal(reader, "Monto1"),
                ReadString(reader, "CasaApuesta2"),
                ReadString(reader, "Equipo2"),
                ReadDecimal(reader, "Cuota2"),
                ReadDecimal(reader, "Monto2"),
                ReadDecimal(reader, "Banca"),
                ReadString(reader, "Instruccion"),
                ReadDecimal(reader, "GananciaEsperada"),
                ReadDecimal(reader, "TotalNetoGanado"));
        }

        /// <summary>
        /// Lee un texto; vacío si la columna es nula.
        /// </summary>
        private static string ReadString(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
        }

        /// <summary>
        /// Lee un decimal; cero si la columna es nula.
        /// </summary>
        private static decimal ReadDecimal(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0m : Convert.ToDecimal(reader.GetValue(ordinal));
        }

        /// <summary>
        /// Lee un bit; falso si la columna es nula.
        /// </summary>
        private static bool ReadBoolean(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return !reader.IsDBNull(ordinal) && reader.GetBoolean(ordinal);
        }

        /// <summary>
        /// Lee una fecha local; nula si la columna es nula.
        /// </summary>
        private static DateTime? ReadDateTime(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            var value = reader.GetDateTime(ordinal);
            return DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
        }

        /// <summary>
        /// Proyecta la entidad de dominio al JSON esperado por spr_ArbitrageOpportunities_BulkLoad.
        /// </summary>
        private static BulkOpportunityPayload MapBulkOpportunity(ArbitrageOpportunity opportunity)
        {
            return new BulkOpportunityPayload
            {
                Id = opportunity.Id,
                DetectedAt = opportunity.DetectedAt,
                EstimatedNetProfitPercent = opportunity.EstimatedNetProfitPercent,
                EvAvailable = opportunity.EvAvailable,
                EvPercentage = opportunity.EvPercentage,
                EventId = opportunity.EventId,
                EventName = opportunity.EventName,
                ImpliedTotal = opportunity.ImpliedTotal,
                IsAlternateLine = opportunity.IsAlternateLine,
                IsLive = opportunity.IsLive,
                IsPlayerProp = opportunity.IsPlayerProp,
                League = opportunity.League,
                LeagueLabel = opportunity.LeagueLabel,
                Line = opportunity.Line,
                LineUnchanged = opportunity.LineUnchanged,
                MarketLabel = opportunity.MarketLabel,
                MarketType = opportunity.MarketType,
                OldestOddsAgeSeconds = opportunity.OldestOddsAgeSeconds,
                PlayerName = opportunity.PlayerName,
                PossiblyStale = opportunity.PossiblyStale,
                ProfitPercent = opportunity.ProfitPercent,
                Sport = opportunity.Sport,
                StartTime = opportunity.StartTime,
                StatCategory = opportunity.StatCategory,
                Warnings = opportunity.Warnings,
                Legs = opportunity.Legs.Select(MapBulkLeg).ToArray()
            };
        }

        /// <summary>
        /// Proyecta una pata al JSON esperado por spr_ArbitrageOpportunities_BulkLoad.
        /// </summary>
        private static BulkLegPayload MapBulkLeg(ArbitrageOpportunityLeg leg)
        {
            return new BulkLegPayload
            {
                Id = leg.Id,
                DeepLink = leg.DeepLink,
                ExternalEventId = leg.ExternalEventId,
                ExternalMarketId = leg.ExternalMarketId,
                Line = leg.Line,
                MarketId = leg.MarketId,
                OddsAmerican = leg.OddsAmerican,
                OddsDecimal = leg.OddsDecimal,
                OddsId = leg.OddsId,
                OddsProbability = leg.OddsProbability,
                Selection = leg.Selection,
                SelectionId = leg.SelectionId,
                Sportsbook = leg.Sportsbook,
                StakePercent = leg.StakePercent
            };
        }

        /// <summary>
        /// Contrato JSON de una oportunidad para la carga masiva.
        /// </summary>
        private sealed class BulkOpportunityPayload
        {
            public required string Id { get; init; }
            public DateTime? DetectedAt { get; init; }
            public decimal? EstimatedNetProfitPercent { get; init; }
            public bool EvAvailable { get; init; }
            public decimal? EvPercentage { get; init; }
            public required string EventId { get; init; }
            public required string EventName { get; init; }
            public decimal? ImpliedTotal { get; init; }
            public bool IsAlternateLine { get; init; }
            public bool IsLive { get; init; }
            public bool IsPlayerProp { get; init; }
            public required string League { get; init; }
            public string? LeagueLabel { get; init; }
            public decimal? Line { get; init; }
            public bool LineUnchanged { get; init; }
            public string? MarketLabel { get; init; }
            public required string MarketType { get; init; }
            public decimal? OldestOddsAgeSeconds { get; init; }
            public string? PlayerName { get; init; }
            public bool PossiblyStale { get; init; }
            public decimal? ProfitPercent { get; init; }
            public required string Sport { get; init; }
            public DateTime? StartTime { get; init; }
            public string? StatCategory { get; init; }
            public string? Warnings { get; init; }
            public required IReadOnlyList<BulkLegPayload> Legs { get; init; }
        }

        /// <summary>
        /// Contrato JSON de una pata para la carga masiva.
        /// </summary>
        private sealed class BulkLegPayload
        {
            public required string Id { get; init; }
            public string? DeepLink { get; init; }
            public string? ExternalEventId { get; init; }
            public string? ExternalMarketId { get; init; }
            public decimal? Line { get; init; }
            public string? MarketId { get; init; }
            public decimal? OddsAmerican { get; init; }
            public decimal? OddsDecimal { get; init; }
            public string? OddsId { get; init; }
            public decimal? OddsProbability { get; init; }
            public required string Selection { get; init; }
            public string? SelectionId { get; init; }
            public required string Sportsbook { get; init; }
            public decimal? StakePercent { get; init; }
        }
    }
}
