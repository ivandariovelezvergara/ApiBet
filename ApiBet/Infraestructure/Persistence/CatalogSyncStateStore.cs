using Infraestructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence
{
    /// <summary>
    /// Operaciones compartidas sobre el estado de sincronización de catálogos.
    /// </summary>
    internal static class CatalogSyncStateStore
    {
        /// <summary>
        /// Inserta o actualiza la fila de metadatos de un catálogo.
        /// </summary>
        /// <param name="dbContext">Contexto de SQL Server.</param>
        /// <param name="syncStateId">Identificador de la fila de catálogo.</param>
        /// <param name="sourceUpdatedAt">Fecha de actualización informada por SharpAPI.</param>
        /// <param name="syncedAtUtc">Fecha local de la persistencia.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        public static async Task UpsertAsync(
            AppDbContext dbContext,
            int syncStateId,
            DateTime? sourceUpdatedAt,
            DateTime syncedAtUtc,
            CancellationToken cancellationToken)
        {
            var syncState = await dbContext.CatalogSyncStates
                .FirstOrDefaultAsync(state => state.Id == syncStateId, cancellationToken);

            if (syncState is null)
            {
                dbContext.CatalogSyncStates.Add(new CatalogSyncState
                {
                    Id = syncStateId,
                    SourceUpdatedAt = sourceUpdatedAt,
                    LastSyncedAtUtc = syncedAtUtc
                });
                return;
            }

            syncState.SourceUpdatedAt = sourceUpdatedAt;
            syncState.LastSyncedAtUtc = syncedAtUtc;
        }
    }
}
