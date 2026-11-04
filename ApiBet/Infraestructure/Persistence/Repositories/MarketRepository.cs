using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Repositories
{
    /// <summary>
    /// Persistencia SQL Server del catálogo de mercados.
    /// </summary>
    internal sealed class MarketRepository : IMarketRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Inicializa el repositorio con el contexto de SQL Server.
        /// </summary>
        /// <param name="dbContext">Contexto de Entity Framework.</param>
        public MarketRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task SaveCatalogAsync(MarketCatalog catalog, CancellationToken cancellationToken)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.MarketSports.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.Markets.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.Markets.AddRangeAsync(catalog.Markets, cancellationToken);

            await CatalogSyncStateStore.UpsertAsync(
                _dbContext,
                CatalogSyncKeys.Markets,
                catalog.SourceUpdatedAt,
                catalog.LastSyncedAtUtc ?? DateTime.Now,
                cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<MarketCatalog> GetCatalogAsync(CancellationToken cancellationToken)
        {
            var markets = await _dbContext.Markets
                .AsNoTracking()
                .Include(market => market.Sports)
                .OrderByDescending(market => market.EventCount)
                .ThenBy(market => market.Name)
                .ToListAsync(cancellationToken);

            var syncState = await _dbContext.CatalogSyncStates
                .AsNoTracking()
                .FirstOrDefaultAsync(state => state.Id == CatalogSyncKeys.Markets, cancellationToken);

            return new MarketCatalog(
                markets,
                syncState?.SourceUpdatedAt,
                syncState?.LastSyncedAtUtc);
        }
    }
}
