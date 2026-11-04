using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de mapeo de <see cref="Sportsbook"/> hacia SQL Server.
    /// </summary>
    internal sealed class SportsbookConfiguration : IEntityTypeConfiguration<Sportsbook>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Sportsbook> builder)
        {
            builder.ToTable("Sportsbooks");
            builder.HasKey(sportsbook => sportsbook.Id);

            builder.Property(sportsbook => sportsbook.Id)
                .HasMaxLength(128);

            builder.Property(sportsbook => sportsbook.DisplayName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(sportsbook => sportsbook.ShortName)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(sportsbook => sportsbook.Category)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(sportsbook => sportsbook.Status)
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(sportsbook => sportsbook.RequiresTier)
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(sportsbook => sportsbook.LastUpdate);

            builder.HasMany(sportsbook => sportsbook.Regions)
                .WithOne()
                .HasForeignKey(region => region.SportsbookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(sportsbook => sportsbook.Status);
            builder.HasIndex(sportsbook => sportsbook.Category);
        }
    }
}
