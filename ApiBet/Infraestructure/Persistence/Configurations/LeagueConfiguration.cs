using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de mapeo de <see cref="League"/> hacia SQL Server.
    /// </summary>
    internal sealed class LeagueConfiguration : IEntityTypeConfiguration<League>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<League> builder)
        {
            builder.ToTable("Leagues");
            builder.HasKey(league => league.Id);

            builder.Property(league => league.Id)
                .HasMaxLength(256);

            builder.Property(league => league.DisplayName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(league => league.SportId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(league => league.EventCount)
                .IsRequired();

            builder.Property(league => league.LiveCount)
                .IsRequired();

            builder.HasIndex(league => league.SportId);
        }
    }
}
