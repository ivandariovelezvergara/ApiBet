using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de mapeo de <see cref="Sport"/> hacia SQL Server.
    /// </summary>
    internal sealed class SportConfiguration : IEntityTypeConfiguration<Sport>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Sport> builder)
        {
            builder.ToTable("Sports");
            builder.HasKey(sport => sport.Id);

            builder.Property(sport => sport.Id)
                .HasMaxLength(64);

            builder.Property(sport => sport.Name)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(sport => sport.EventCount)
                .IsRequired();

            builder.Property(sport => sport.LiveCount)
                .IsRequired();

            builder.HasMany(sport => sport.Leagues)
                .WithOne()
                .HasForeignKey(league => league.SportId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
