using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhoneHub.Core.Entities;

namespace PhoneHub.Infrastructure.Data.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> entity)
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("invoices");

            // RN-12: una venta solo puede tener una factura
            entity.HasIndex(e => e.SaleId)
                .IsUnique()
                .HasDatabaseName("UQ_Invoice_SaleId");

            // RN-13: número de factura único
            entity.HasIndex(e => e.InvoiceNumber)
                .IsUnique()
                .HasDatabaseName("UQ_Invoice_Number");

            entity.Property(e => e.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.IssuedAt)
                .IsRequired()
                .HasColumnType("datetime");

            entity.HasOne(d => d.Sale)
                .WithMany()
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_Sale");
        }
    }
}
