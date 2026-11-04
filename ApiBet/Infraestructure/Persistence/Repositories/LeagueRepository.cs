using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Persistencia SQL Server del catálogo de ligas.
    /// </summary>
    internal sealed class LeagueRepository : ILeagueRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Inicializa el repositorio con el contexto de SQL Server.
        /// </summary>
        /// <param name="dbContext">Contexto de Entity Framework.</param>
        public LeagueRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task SaveCatalogAsync(LeagueCatalog catalog, CancellationToken cancellationToken)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.Leagues.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.Leagues.AddRangeAsync(catalog.Leagues, cancellationToken);

            await CatalogSyncStateStore.UpsertAsync(
                _dbContext,
                CatalogSyncKeys.Leagues,
                catalog.SourceUpdatedAt,
                catalog.LastSyncedAtUtc ?? DateTime.Now,
                cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<LeagueCatalog> GetCatalogAsync(CancellationToken cancellationToken)
        {
            var leagues = await _dbContext.Leagues
                .AsNoTracking()
                .OrderByDescending(league => league.EventCount)
                .ThenBy(league => league.DisplayName)
                .ToListAsync(cancellationToken);

            var syncState = await _dbContext.CatalogSyncStates
                .AsNoTracking()
                .FirstOrDefaultAsync(state => state.Id == CatalogSyncKeys.Leagues, cancellationToken);

            return new LeagueCatalog(
                leagues,
                syncState?.SourceUpdatedAt,
                syncState?.LastSyncedAtUtc);
        }
    }
}
