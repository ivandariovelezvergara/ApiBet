using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de mapeo de <see cref="MarketOdd"/> hacia SQL Server.
    /// </summary>
    internal sealed class MarketOddConfiguration : IEntityTypeConfiguration<MarketOdd>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<MarketOdd> builder)
        {
            builder.ToTable("MarketOdds");
            builder.HasKey(odd => odd.Id);

            builder.Property(odd => odd.Id).HasMaxLength(64);
            builder.Property(odd => odd.Sportsbook).IsRequired().HasMaxLength(128);
            builder.Property(odd => odd.EventId).IsRequired().HasMaxLength(256);
            builder.Property(odd => odd.EventUuid).HasMaxLength(64);
            builder.Property(odd => odd.Sport).IsRequired().HasMaxLength(64);
            builder.Property(odd => odd.League).IsRequired().HasMaxLength(256);
            builder.Property(odd => odd.HomeTeam).HasMaxLength(256);
            builder.Property(odd => odd.AwayTeam).HasMaxLength(256);
            builder.Property(odd => odd.MarketType).IsRequired().HasMaxLength(128);
            builder.Property(odd => odd.Selection).IsRequired().HasMaxLength(256);
            builder.Property(odd => odd.SelectionType).HasMaxLength(64);
            builder.Property(odd => odd.TeamSide).HasMaxLength(64);
            builder.Property(odd => odd.OddsAmerican).HasPrecision(18, 8);
            builder.Property(odd => odd.OddsDecimal).HasPrecision(18, 8);
            builder.Property(odd => odd.OddsProbability).HasPrecision(18, 8);
            builder.Property(odd => odd.Line).HasPrecision(18, 8);
            builder.Property(odd => odd.Timestamp)
                .HasColumnName("Timestamp");
            builder.Property(odd => odd.DeepLink).HasMaxLength(512);
            builder.Property(odd => odd.MarketId).HasMaxLength(128);
            builder.Property(odd => odd.SelectionId).HasMaxLength(128);
            builder.Property(odd => odd.ExternalMarketId).HasMaxLength(128);
            builder.Property(odd => odd.SeasonType).HasMaxLength(64);
            builder.Property(odd => odd.HomeId).HasMaxLength(128);
            builder.Property(odd => odd.HomeName).HasMaxLength(256);
            builder.Property(odd => odd.HomeLogo).HasMaxLength(512);
            builder.Property(odd => odd.HomeAbbreviation).HasMaxLength(32);
            builder.Property(odd => odd.HomeCity).HasMaxLength(128);
            builder.Property(odd => odd.HomeMascot).HasMaxLength(128);
            builder.Property(odd => odd.HomeConference).HasMaxLength(64);
            builder.Property(odd => odd.HomeDivision).HasMaxLength(128);
            builder.Property(odd => odd.AwayId).HasMaxLength(128);
            builder.Property(odd => odd.AwayName).HasMaxLength(256);
            builder.Property(odd => odd.AwayLogo).HasMaxLength(512);
            builder.Property(odd => odd.AwayAbbreviation).HasMaxLength(32);
            builder.Property(odd => odd.AwayCity).HasMaxLength(128);
            builder.Property(odd => odd.AwayMascot).HasMaxLength(128);
            builder.Property(odd => odd.AwayConference).HasMaxLength(64);
            builder.Property(odd => odd.AwayDivision).HasMaxLength(128);
            builder.Property(odd => odd.SportRefId).HasMaxLength(64);
            builder.Property(odd => odd.SportRefName).HasMaxLength(128);
            builder.Property(odd => odd.LeagueRefId).HasMaxLength(256);
            builder.Property(odd => odd.LeagueRefLabel).HasMaxLength(256);
            builder.Property(odd => odd.MarketRefId).HasMaxLength(128);
            builder.Property(odd => odd.MarketRefLabel).HasMaxLength(128);
            builder.Property(odd => odd.SportsbookRefId).HasMaxLength(128);
            builder.Property(odd => odd.SportsbookRefName).HasMaxLength(128);
            builder.Property(odd => odd.CompetitionRefId).HasMaxLength(256);
            builder.Property(odd => odd.CompetitionRefLabel).HasMaxLength(256);
            builder.Property(odd => odd.ExternalEventId).HasMaxLength(64);
            builder.Property(odd => odd.CreatedAtUtc)
                .HasDefaultValueSql("SYSDATETIME()");

            builder.HasIndex(odd => odd.Sport);
            builder.HasIndex(odd => odd.EventId);
            builder.HasIndex(odd => odd.League);
            builder.HasIndex(odd => odd.MarketType);
            builder.HasIndex(odd => odd.Sportsbook);
            builder.HasIndex(odd => odd.EventStartTime);
            builder.HasIndex(odd => odd.CreatedAtUtc);
        }
    }
}
