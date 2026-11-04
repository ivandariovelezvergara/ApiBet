using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Persistencia SQL Server de las mejores cuotas.
    /// </summary>
    internal sealed class BestOddsRepository : IBestOddsRepository
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Inicializa el repositorio con el contexto de SQL Server.
        /// </summary>
        public BestOddsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task ClearAllAsync(CancellationToken cancellationToken)
        {
            await _dbContext.BestOddBooks.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.BestOdds.ExecuteDeleteAsync(cancellationToken);
            _dbContext.ChangeTracker.Clear();
        }

        /// <inheritdoc />
        public async Task<(int OddCount, int BookCount)> SavePageAsync(
            IReadOnlyList<BestOdd> odds,
            DateTime? sourceUpdatedAt,
            CancellationToken cancellationToken)
        {
            if (odds.Count == 0)
            {
                return (0, 0);
            }

            var payload = odds.Select(MapBulkOdd).ToArray();
            var json = JsonSerializer.Serialize(payload, BulkJson.Options);

            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "dbo.spr_BestOdds_BulkLoad";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 180;
            AddParameter(command, "@Json", json);

            var oddCount = 0;
            var bookCount = 0;
            await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    oddCount = Convert.ToInt32(reader.GetValue(0));
                    bookCount = Convert.ToInt32(reader.GetValue(1));
                }
            }

            await CatalogSyncStateStore.UpsertAsync(
                _dbContext,
                CatalogSyncKeys.BestOdds,
                sourceUpdatedAt,
                DateTime.Now,
                cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            _dbContext.ChangeTracker.Clear();

            return (oddCount, bookCount);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<BestOddRow>> GetAsync(
            string? sport,
            CancellationToken cancellationToken)
        {
            var sportFilter = string.IsNullOrWhiteSpace(sport) ? null : sport.Trim();

            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "dbo.spr_BestOdds";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 180;
            AddParameter(command, "@sport", sportFilter is null ? DBNull.Value : sportFilter);

            var json = new StringBuilder();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                if (!reader.IsDBNull(0))
                {
                    json.Append(reader.GetString(0));
                }
            }

            if (json.Length == 0)
            {
                return Array.Empty<BestOddRow>();
            }

            var rows = JsonSerializer.Deserialize<List<BestOddRow>>(json.ToString(), JsonOptions);
            return rows ?? new List<BestOddRow>();
        }

        /// <inheritdoc />
        public async Task<ValueBetCatalog> GetValueBetsAsync(
            ValueBetQuery query,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(query);

            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "dbo.spr_BestOdds_ValueBets";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 180;
            AddParameter(command, "@Bankroll", query.Bankroll);
            AddParameter(command, "@MaxStakePercent", query.MaxStakePercent);
            AddParameter(command, "@MaxTotalExposurePercent", query.MaxTotalExposurePercent);
            AddParameter(command, "@KellyFraction", query.KellyFraction);
            AddParameter(command, "@MaxPicks", query.MaxPicks);
            AddParameter(command, "@MinExpectedValue", query.MinExpectedValue);
            AddParameter(command, "@MaxExpectedValue", query.MaxExpectedValue);
            AddParameter(command, "@MinOdds", query.MinOdds);
            AddParameter(command, "@MaxOdds", query.MaxOdds);
            AddParameter(command, "@MinMinutesToStart", query.MinMinutesToStart);
            AddParameter(command, "@MaxMinutesToStart", query.MaxMinutesToStart);
            AddParameter(command, "@StakeRoundToCop", query.StakeRoundToCop);
            AddParameter(command, "@MinStakeCop", query.MinStakeCop);
            AddParameter(command, "@IsMainLine", query.IsMainLine);

            var picks = new List<ValueBetPick>();
            var summaries = new List<ValueBetSummary>();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                picks.Add(MapPick(reader));
            }

            if (await reader.NextResultAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    summaries.Add(MapSummary(reader));
                }
            }

            return new ValueBetCatalog(picks, summaries);
        }

        /// <summary>
        /// Agrega un parámetro al comando ADO.NET.
        /// </summary>
        private static void AddParameter(DbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            parameter.DbType = value switch
            {
                string => DbType.String,
                decimal => DbType.Decimal,
                int => DbType.Int32,
                bool => DbType.Boolean,
                _ => parameter.DbType
            };
            if (value is string)
            {
                parameter.Size = -1;
            }

            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Lee un pick del primer result set de dbo.spr_BestOdds_ValueBets.
        /// </summary>
        private static ValueBetPick MapPick(DbDataReader reader)
        {
            return new ValueBetPick
            {
                Pick = Convert.ToInt32(ReadValue(reader, "Pick")),
                Senal = ReadString(reader, "Senal"),
                Deporte = ReadString(reader, "Deporte"),
                Liga = ReadString(reader, "Liga"),
                Partido = ReadString(reader, "Partido"),
                MinutosFaltantes = Convert.ToInt32(ReadValue(reader, "MinutosFaltantes")),
                Mercado = ReadString(reader, "Mercado"),
                ApostarA = ReadString(reader, "ApostarA"),
                Linea = ReadNullableDecimal(reader, "Linea"),
                Casa = ReadString(reader, "Casa"),
                CuotaActual = ReadDecimal(reader, "CuotaActual"),
                CuotaMinimaRentable = ReadDecimal(reader, "CuotaMinimaRentable"),
                ProbConsensoPct = ReadDecimal(reader, "ProbConsensoPct"),
                ProbImplicitaPct = ReadDecimal(reader, "ProbImplicitaPct"),
                EvPct = ReadDecimal(reader, "EvPct"),
                KellyAplicadoPct = ReadDecimal(reader, "KellyAplicadoPct"),
                StakeCop = ReadDecimal(reader, "StakeCop"),
                GananciaEsperadaCop = ReadDecimal(reader, "GananciaEsperadaCop"),
                RiesgoAcumuladoCop = ReadDecimal(reader, "RiesgoAcumuladoCop"),
                Instruccion = ReadString(reader, "Instruccion")
            };
        }

        /// <summary>
        /// Lee un resumen del segundo result set de dbo.spr_BestOdds_ValueBets.
        /// </summary>
        private static ValueBetSummary MapSummary(DbDataReader reader)
        {
            return new ValueBetSummary
            {
                Deporte = ReadString(reader, "Deporte"),
                Liga = ReadString(reader, "Liga"),
                PartidosConValor = Convert.ToInt32(ReadValue(reader, "PartidosConValor")),
                PicksTotales = Convert.ToInt32(ReadValue(reader, "PicksTotales")),
                SurebetsDetectadas = Convert.ToInt32(ReadValue(reader, "SurebetsDetectadas") ?? 0),
                EvMedioPct = ReadDecimal(reader, "EvMedioPct"),
                CuotaMedia = ReadDecimal(reader, "CuotaMedia")
            };
        }

        private static string ReadString(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? string.Empty : Convert.ToString(reader.GetValue(ordinal)) ?? string.Empty;
        }

        private static decimal ReadDecimal(DbDataReader reader, string column)
        {
            return ReadNullableDecimal(reader, column) ?? 0m;
        }

        private static decimal? ReadNullableDecimal(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : Convert.ToDecimal(reader.GetValue(ordinal));
        }

        private static object? ReadValue(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal);
        }

        /// <summary>
        /// Proyecta la entidad de dominio al JSON esperado por spr_BestOdds_BulkLoad.
        /// </summary>
        private static BulkOddPayload MapBulkOdd(BestOdd odd)
        {
            return new BulkOddPayload
            {
                Id = odd.Id,
                EventId = odd.EventId,
                EventName = odd.EventName,
                Sport = odd.Sport,
                League = odd.League,
                MarketType = odd.MarketType,
                Selection = odd.Selection,
                Line = odd.Line,
                IsMainLine = odd.IsMainLine,
                BestAmerican = odd.BestAmerican,
                BestDecimal = odd.BestDecimal,
                BestProbability = odd.BestProbability,
                ConsensusAmerican = odd.ConsensusAmerican,
                ConsensusDecimal = odd.ConsensusDecimal,
                ConsensusProbability = odd.ConsensusProbability,
                MarketHold = odd.MarketHold,
                BestBook = odd.BestBook,
                Timestamp = odd.Timestamp,
                PlayerName = odd.PlayerName,
                StatCategory = odd.StatCategory,
                Books = odd.Books.Select(MapBulkBook).ToArray()
            };
        }

        /// <summary>
        /// Proyecta all_books al JSON esperado por spr_BestOdds_BulkLoad.
        /// </summary>
        private static BulkBookPayload MapBulkBook(BestOddBook book)
        {
            return new BulkBookPayload
            {
                Id = book.Id,
                Book = book.Book,
                Sportsbook = book.Sportsbook,
                American = book.American,
                Decimal = book.DecimalOdds,
                Edge = book.Edge
            };
        }

        /// <summary>
        /// Contrato JSON de una fila de BestOdds para la carga masiva.
        /// </summary>
        private sealed class BulkOddPayload
        {
            public required string Id { get; init; }
            public required string EventId { get; init; }
            public required string EventName { get; init; }
            public required string Sport { get; init; }
            public required string League { get; init; }
            public required string MarketType { get; init; }
            public required string Selection { get; init; }
            public decimal? Line { get; init; }
            public bool IsMainLine { get; init; }
            public decimal? BestAmerican { get; init; }
            public decimal? BestDecimal { get; init; }
            public decimal? BestProbability { get; init; }
            public decimal? ConsensusAmerican { get; init; }
            public decimal? ConsensusDecimal { get; init; }
            public decimal? ConsensusProbability { get; init; }
            public decimal? MarketHold { get; init; }
            public string? BestBook { get; init; }
            public DateTime? Timestamp { get; init; }
            public string? PlayerName { get; init; }
            public string? StatCategory { get; init; }
            public required IReadOnlyList<BulkBookPayload> Books { get; init; }
        }

        /// <summary>
        /// Contrato JSON de una fila de BestOddBooks para la carga masiva.
        /// </summary>
        private sealed class BulkBookPayload
        {
            public required string Id { get; init; }
            public required string Book { get; init; }
            public required string Sportsbook { get; init; }
            public decimal? American { get; init; }

            [JsonPropertyName("decimal")]
            public decimal? Decimal { get; init; }

            public decimal? Edge { get; init; }
        }
    }
}
