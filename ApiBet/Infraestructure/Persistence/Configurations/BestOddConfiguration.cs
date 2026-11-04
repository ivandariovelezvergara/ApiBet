using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de mapeo de <see cref="BestOdd"/> hacia SQL Server.
    /// </summary>
    internal sealed class BestOddConfiguration : IEntityTypeConfiguration<BestOdd>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<BestOdd> builder)
        {
            builder.ToTable("BestOdds");
            builder.HasKey(odd => odd.Id);

            builder.Property(odd => odd.Id)
                .HasMaxLength(64);

            builder.Property(odd => odd.EventId)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(odd => odd.EventName)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(odd => odd.Sport)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(odd => odd.League)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(odd => odd.MarketType)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(odd => odd.Selection)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(odd => odd.Line).HasPrecision(18, 8);
            builder.Property(odd => odd.BestAmerican).HasPrecision(18, 8);
            builder.Property(odd => odd.BestDecimal).HasPrecision(18, 8);
            builder.Property(odd => odd.BestProbability).HasPrecision(18, 8);
            builder.Property(odd => odd.ConsensusAmerican).HasPrecision(18, 8);
            builder.Property(odd => odd.ConsensusDecimal).HasPrecision(18, 8);
            builder.Property(odd => odd.ConsensusProbability).HasPrecision(18, 8);
            builder.Property(odd => odd.MarketHold).HasPrecision(18, 8);

            builder.Property(odd => odd.BestBook)
                .HasMaxLength(128);

            builder.Property(odd => odd.Timestamp)
                .HasColumnName("Timestamp");

            builder.Property(odd => odd.CreatedAtUtc)
                .HasDefaultValueSql("SYSDATETIME()");

            builder.HasMany(odd => odd.Books)
                .WithOne()
                .HasForeignKey(book => book.BestOddId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(odd => odd.Books)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            builder.HasIndex(odd => odd.Sport);
            builder.HasIndex(odd => odd.EventId);
            builder.HasIndex(odd => odd.League);
            builder.HasIndex(odd => odd.MarketType);
            builder.HasIndex(odd => odd.BestBook);
            builder.HasIndex(odd => odd.CreatedAtUtc);
        }
    }
}
