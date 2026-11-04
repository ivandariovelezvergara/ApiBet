using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de mapeo de <see cref="SportLeague"/> hacia SQL Server.
    /// </summary>
    internal sealed class SportLeagueConfiguration : IEntityTypeConfiguration<SportLeague>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<SportLeague> builder)
        {
            builder.ToTable("SportLeagues");
            builder.HasKey(league => new { league.SportId, league.Code });

            builder.Property(league => league.SportId)
                .HasMaxLength(64);

            builder.Property(league => league.Code)
                .HasMaxLength(128);
        }
    }
}
