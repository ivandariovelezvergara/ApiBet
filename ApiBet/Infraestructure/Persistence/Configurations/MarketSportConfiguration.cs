using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de mapeo de <see cref="MarketSport"/> hacia SQL Server.
    /// </summary>
    internal sealed class MarketSportConfiguration : IEntityTypeConfiguration<MarketSport>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<MarketSport> builder)
        {
            builder.ToTable("MarketSports");
            builder.HasKey(sport => new { sport.MarketId, sport.SportId });

            builder.Property(sport => sport.MarketId)
                .HasMaxLength(128);

            builder.Property(sport => sport.SportId)
                .HasMaxLength(64);
        }
    }
}
