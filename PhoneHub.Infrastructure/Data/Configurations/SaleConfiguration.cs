using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhoneHub.Core.Entities;

namespace PhoneHub.Infrastructure.Data.Configurations
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Date)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(e => e.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(e => e.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(e => e.IsActive)
                .HasColumnType("bit")
                .IsRequired();

            // Relación con el Producto (Requerimiento 3)
            builder.HasOne(d => d.Product)
                .WithMany(p => p.Sales)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Relación con el Usuario/Vendedor (Requerimiento 7)
            builder.HasOne(d => d.User)
                .WithMany(p => p.Sales)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}