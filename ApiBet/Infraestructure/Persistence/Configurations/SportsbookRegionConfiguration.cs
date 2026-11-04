using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de mapeo de <see cref="SportsbookRegion"/> hacia SQL Server.
    /// </summary>
    internal sealed class SportsbookRegionConfiguration : IEntityTypeConfiguration<SportsbookRegion>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<SportsbookRegion> builder)
        {
            builder.ToTable("SportsbookRegions");
            builder.HasKey(region => new { region.SportsbookId, region.Code });

            builder.Property(region => region.SportsbookId)
                .HasMaxLength(128);

            builder.Property(region => region.Code)
                .HasMaxLength(32);
        }
    }
}
