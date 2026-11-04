using Infraestructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de mapeo de <see cref="CatalogSyncState"/> hacia SQL Server.
    /// </summary>
    internal sealed class CatalogSyncStateConfiguration : IEntityTypeConfiguration<CatalogSyncState>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<CatalogSyncState> builder)
        {
            builder.ToTable("CatalogSyncStates");
            builder.HasKey(state => state.Id);

            builder.Property(state => state.Id)
                .ValueGeneratedNever();

            builder.Property(state => state.SourceUpdatedAt);

            builder.Property(state => state.LastSyncedAtUtc)
                .IsRequired();
        }
    }
}
