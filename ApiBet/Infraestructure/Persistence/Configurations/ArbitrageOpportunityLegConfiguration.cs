using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de mapeo de <see cref="ArbitrageOpportunityLeg"/> hacia SQL Server.
    /// </summary>
    internal sealed class ArbitrageOpportunityLegConfiguration : IEntityTypeConfiguration<ArbitrageOpportunityLeg>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<ArbitrageOpportunityLeg> builder)
        {
            builder.ToTable("ArbitrageOpportunityLegs");
            builder.HasKey(leg => leg.Id);

            builder.Property(leg => leg.Id).HasMaxLength(64);
            builder.Property(leg => leg.ArbitrageId).IsRequired().HasMaxLength(64);
            builder.Property(leg => leg.DeepLink).HasMaxLength(1024);
            builder.Property(leg => leg.ExternalEventId).HasMaxLength(256);
            builder.Property(leg => leg.ExternalMarketId).HasMaxLength(256);
            builder.Property(leg => leg.Line).HasPrecision(18, 8);
            builder.Property(leg => leg.MarketId).HasMaxLength(256);
            builder.Property(leg => leg.OddsAmerican).HasPrecision(18, 8);
            builder.Property(leg => leg.OddsDecimal).HasPrecision(18, 8);
            builder.Property(leg => leg.OddsId).HasMaxLength(64);
            builder.Property(leg => leg.OddsProbability).HasPrecision(18, 8);
            builder.Property(leg => leg.Selection).IsRequired().HasMaxLength(512);
            builder.Property(leg => leg.SelectionId).HasMaxLength(256);
            builder.Property(leg => leg.Sportsbook).IsRequired().HasMaxLength(128);
            builder.Property(leg => leg.StakePercent).HasPrecision(18, 8);

            builder.HasIndex(leg => leg.ArbitrageId);
            builder.HasIndex(leg => leg.Sportsbook);
        }
    }
}
