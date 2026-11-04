using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Persistencia SQL Server del catálogo de casas de apuestas.
    /// </summary>
    internal sealed class SportsbookRepository : ISportsbookRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Inicializa el repositorio con el contexto de SQL Server.
        /// </summary>
        /// <param name="dbContext">Contexto de Entity Framework.</param>
        public SportsbookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task SaveCatalogAsync(SportsbookCatalog catalog, CancellationToken cancellationToken)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.SportsbookRegions.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.Sportsbooks.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.Sportsbooks.AddRangeAsync(catalog.Sportsbooks, cancellationToken);

            await CatalogSyncStateStore.UpsertAsync(
                _dbContext,
                CatalogSyncKeys.Sportsbooks,
                catalog.SourceUpdatedAt,
                catalog.LastSyncedAtUtc ?? DateTime.Now,
                cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<SportsbookCatalog> GetCatalogAsync(CancellationToken cancellationToken)
        {
            var sportsbooks = await _dbContext.Sportsbooks
                .AsNoTracking()
                .Include(sportsbook => sportsbook.Regions)
                .OrderByDescending(sportsbook => sportsbook.EventCount)
                .ThenBy(sportsbook => sportsbook.DisplayName)
                .ToListAsync(cancellationToken);

            var syncState = await _dbContext.CatalogSyncStates
                .AsNoTracking()
                .FirstOrDefaultAsync(state => state.Id == CatalogSyncKeys.Sportsbooks, cancellationToken);

            return new SportsbookCatalog(
                sportsbooks,
                syncState?.SourceUpdatedAt,
                syncState?.LastSyncedAtUtc);
        }
    }
}
