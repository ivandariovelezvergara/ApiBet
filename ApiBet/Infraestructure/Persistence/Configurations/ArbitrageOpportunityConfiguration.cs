using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de mapeo de <see cref="ArbitrageOpportunity"/> hacia SQL Server.
    /// </summary>
    internal sealed class ArbitrageOpportunityConfiguration : IEntityTypeConfiguration<ArbitrageOpportunity>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<ArbitrageOpportunity> builder)
        {
            builder.ToTable("ArbitrageOpportunities");
            builder.HasKey(opportunity => opportunity.Id);

            builder.Property(opportunity => opportunity.Id).HasMaxLength(64);
            builder.Property(opportunity => opportunity.DetectedAt);
            builder.Property(opportunity => opportunity.EstimatedNetProfitPercent).HasPrecision(18, 8);
            builder.Property(opportunity => opportunity.EvPercentage).HasPrecision(18, 8);
            builder.Property(opportunity => opportunity.EventId).IsRequired().HasMaxLength(256);
            builder.Property(opportunity => opportunity.EventName).IsRequired().HasMaxLength(512);
            builder.Property(opportunity => opportunity.ImpliedTotal).HasPrecision(18, 8);
            builder.Property(opportunity => opportunity.League).IsRequired().HasMaxLength(256);
            builder.Property(opportunity => opportunity.LeagueLabel).HasMaxLength(256);
            builder.Property(opportunity => opportunity.Line).HasPrecision(18, 8);
            builder.Property(opportunity => opportunity.MarketLabel).HasMaxLength(256);
            builder.Property(opportunity => opportunity.MarketType).IsRequired().HasMaxLength(128);
            builder.Property(opportunity => opportunity.OldestOddsAgeSeconds).HasPrecision(18, 8);
            builder.Property(opportunity => opportunity.PlayerName).HasMaxLength(256);
            builder.Property(opportunity => opportunity.ProfitPercent).HasPrecision(18, 8);
            builder.Property(opportunity => opportunity.Sport).IsRequired().HasMaxLength(64);
            builder.Property(opportunity => opportunity.StartTime);
            builder.Property(opportunity => opportunity.StatCategory).HasMaxLength(128);
            builder.Property(opportunity => opportunity.Warnings).HasMaxLength(1024);
            builder.Property(opportunity => opportunity.CreatedAtUtc)
                .HasDefaultValueSql("SYSDATETIME()");

            builder.HasMany(opportunity => opportunity.Legs)
                .WithOne()
                .HasForeignKey(leg => leg.ArbitrageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(opportunity => opportunity.Legs)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            builder.HasIndex(opportunity => opportunity.Sport);
            builder.HasIndex(opportunity => opportunity.EventId);
            builder.HasIndex(opportunity => opportunity.League);
            builder.HasIndex(opportunity => opportunity.MarketType);
            builder.HasIndex(opportunity => opportunity.ProfitPercent);
            builder.HasIndex(opportunity => opportunity.CreatedAtUtc);
        }
    }
}
