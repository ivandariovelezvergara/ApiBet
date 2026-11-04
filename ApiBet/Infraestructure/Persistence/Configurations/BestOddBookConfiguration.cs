using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de mapeo de <see cref="BestOddBook"/> hacia SQL Server.
    /// </summary>
    internal sealed class BestOddBookConfiguration : IEntityTypeConfiguration<BestOddBook>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<BestOddBook> builder)
        {
            builder.ToTable("BestOddBooks");
            builder.HasKey(book => book.Id);

            builder.Property(book => book.Id)
                .HasMaxLength(64);

            builder.Property(book => book.BestOddId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(book => book.Book)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(book => book.Sportsbook)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(book => book.American).HasPrecision(18, 8);
            builder.Property(book => book.DecimalOdds)
                .HasColumnName("Decimal")
                .HasPrecision(18, 8);
            builder.Property(book => book.Edge).HasPrecision(18, 8);

            builder.HasIndex(book => book.BestOddId);
        }
    }
}
