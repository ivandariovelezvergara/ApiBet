using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de mapeo de <see cref="Market"/> hacia SQL Server.
    /// </summary>
    internal sealed class MarketConfiguration : IEntityTypeConfiguration<Market>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Market> builder)
        {
            builder.ToTable("Markets");
            builder.HasKey(market => market.Id);

            builder.Property(market => market.Id)
                .HasMaxLength(128);

            builder.Property(market => market.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(market => market.EventCount)
                .IsRequired();

            builder.HasMany(market => market.Sports)
                .WithOne()
                .HasForeignKey(sport => sport.MarketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
