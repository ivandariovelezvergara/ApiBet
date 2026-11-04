using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Persistencia SQL Server del catálogo de deportes.
    /// </summary>
    internal sealed class SportRepository : ISportRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Inicializa el repositorio con el contexto de SQL Server.
        /// </summary>
        /// <param name="dbContext">Contexto de Entity Framework.</param>
        public SportRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task SaveCatalogAsync(SportCatalog catalog, CancellationToken cancellationToken)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.SportLeagues.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.Sports.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.Sports.AddRangeAsync(catalog.Sports, cancellationToken);

            await CatalogSyncStateStore.UpsertAsync(
                _dbContext,
                CatalogSyncKeys.Sports,
                catalog.SourceUpdatedAt,
                catalog.LastSyncedAtUtc ?? DateTime.Now,
                cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<SportCatalog> GetCatalogAsync(CancellationToken cancellationToken)
        {
            var sports = await _dbContext.Sports
                .AsNoTracking()
                .Include(sport => sport.Leagues)
                .OrderByDescending(sport => sport.EventCount)
                .ThenBy(sport => sport.Name)
                .ToListAsync(cancellationToken);

            var syncState = await _dbContext.CatalogSyncStates
                .AsNoTracking()
                .FirstOrDefaultAsync(state => state.Id == CatalogSyncKeys.Sports, cancellationToken);

            return new SportCatalog(
                sports,
                syncState?.SourceUpdatedAt,
                syncState?.LastSyncedAtUtc);
        }
    }
}
