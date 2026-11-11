using System.Data;
using System.Data.Common;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Persistencia SQL Server de la bitácora de apuestas ejecutadas (dbo.PlacedBets).
    /// </summary>
    internal sealed class PlacedBetRepository : IPlacedBetRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Inicializa el repositorio con el contexto de SQL Server.
        /// </summary>
        public PlacedBetRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<PlacedBet> CreateAsync(NewPlacedBet bet, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(bet);

            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "dbo.spr_PlacedBets_Insert";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 60;

            AddParameter(command, "@BetKind", bet.BetKind);
            AddParameter(command, "@Deporte", bet.Deporte);
            AddParameter(command, "@Liga", bet.Liga);
            AddParameter(command, "@Partido", bet.Partido);
            AddParameter(command, "@Mercado", bet.Mercado);
            AddParameter(command, "@Seleccion", bet.Seleccion);
            AddParameter(command, "@Linea", bet.Linea is null ? DBNull.Value : bet.Linea.Value);
            AddParameter(command, "@Casa", bet.Casa);
            AddParameter(command, "@CuotaTomada", bet.CuotaTomada);
            AddParameter(command, "@CuotaMinimaRentable", bet.CuotaMinimaRentable is null ? DBNull.Value : bet.CuotaMinimaRentable.Value);
            AddParameter(command, "@ProbConsensoTomadaPct", bet.ProbConsensoTomadaPct is null ? DBNull.Value : bet.ProbConsensoTomadaPct.Value);
            AddParameter(command, "@EvEstimadoPct", bet.EvEstimadoPct);
            AddParameter(command, "@StakeCop", bet.StakeCop);
            AddParameter(command, "@GananciaEsperadaCop", bet.GananciaEsperadaCop is null ? DBNull.Value : bet.GananciaEsperadaCop.Value);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return MapBet(reader);
            }

            throw new InvalidOperationException("No se pudo registrar la apuesta en dbo.PlacedBets.");
        }

        /// <inheritdoc />
        public async Task<PlacedBet?> UpdateStatusAsync(int id, string status, CancellationToken cancellationToken)
        {
            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "dbo.spr_PlacedBets_UpdateStatus";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 60;

            AddParameter(command, "@Id", id);
            AddParameter(command, "@Estado", status);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return MapBet(reader);
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<PlacedBetsCatalog> GetCatalogAsync(CancellationToken cancellationToken)
        {
            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "dbo.spr_PlacedBets_Get";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 60;

            var bets = new List<PlacedBet>();
            var summary = new PlacedBetsSummary();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                bets.Add(MapBet(reader));
            }

            if (await reader.NextResultAsync(cancellationToken) && await reader.ReadAsync(cancellationToken))
            {
                summary = MapSummary(reader);
            }

            return new PlacedBetsCatalog(bets, summary);
        }

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

        private static PlacedBet MapBet(DbDataReader reader)
        {
            return new PlacedBet
            {
                Id = Convert.ToInt32(ReadValue(reader, "Id")),
                BetKind = ReadString(reader, "BetKind"),
                Deporte = ReadString(reader, "Deporte"),
                Liga = ReadString(reader, "Liga"),
                Partido = ReadString(reader, "Partido"),
                Mercado = ReadString(reader, "Mercado"),
                Seleccion = ReadString(reader, "Seleccion"),
                Linea = ReadNullableDecimal(reader, "Linea"),
                Casa = ReadString(reader, "Casa"),
                CuotaTomada = ReadDecimal(reader, "CuotaTomada"),
                CuotaMinimaRentable = ReadNullableDecimal(reader, "CuotaMinimaRentable"),
                ProbConsensoTomadaPct = ReadNullableDecimal(reader, "ProbConsensoTomadaPct"),
                EvEstimadoPct = ReadDecimal(reader, "EvEstimadoPct"),
                StakeCop = ReadDecimal(reader, "StakeCop"),
                GananciaEsperadaCop = ReadDecimal(reader, "GananciaEsperadaCop"),
                UltimaCuotaMercado = ReadNullableDecimal(reader, "UltimaCuotaMercado"),
                UltimaProbConsensoPct = ReadNullableDecimal(reader, "UltimaProbConsensoPct"),
                ClvCuotaPct = ReadNullableDecimal(reader, "ClvCuotaPct"),
                Estado = ReadString(reader, "Estado"),
                ProfitRealCop = ReadNullableDecimal(reader, "ProfitRealCop"),
                PlacedAtCol = ReadDateTime(reader, "PlacedAtCol"),
                ClosedAtCol = ReadNullableDateTime(reader, "ClosedAtCol")
            };
        }

        private static PlacedBetsSummary MapSummary(DbDataReader reader)
        {
            return new PlacedBetsSummary
            {
                TotalBets = Convert.ToInt32(ReadValue(reader, "TotalBets") ?? 0),
                PendingBets = Convert.ToInt32(ReadValue(reader, "PendingBets") ?? 0),
                ResolvedBets = Convert.ToInt32(ReadValue(reader, "ResolvedBets") ?? 0),
                WonBets = Convert.ToInt32(ReadValue(reader, "WonBets") ?? 0),
                LostBets = Convert.ToInt32(ReadValue(reader, "LostBets") ?? 0),
                PushBets = Convert.ToInt32(ReadValue(reader, "PushBets") ?? 0),
                TotalStakedCop = ReadDecimal(reader, "TotalStakedCop"),
                ResolvedStakedCop = ReadDecimal(reader, "ResolvedStakedCop"),
                PendingExposureCop = ReadDecimal(reader, "PendingExposureCop"),
                RealizedProfitCop = ReadDecimal(reader, "RealizedProfitCop"),
                ExpectedProfitCop = ReadDecimal(reader, "ExpectedProfitCop"),
                RoiPct = ReadDecimal(reader, "RoiPct"),
                AvgEvPct = ReadDecimal(reader, "AvgEvPct"),
                AvgClvPct = ReadDecimal(reader, "AvgClvPct"),
                PositiveClvRatePct = ReadDecimal(reader, "PositiveClvRatePct")
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

        private static DateTime ReadDateTime(DbDataReader reader, string column)
        {
            return ReadNullableDateTime(reader, column) ?? DateTime.Now;
        }

        private static DateTime? ReadNullableDateTime(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : Convert.ToDateTime(reader.GetValue(ordinal));
        }

        private static object? ReadValue(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal);
        }
    }
}
