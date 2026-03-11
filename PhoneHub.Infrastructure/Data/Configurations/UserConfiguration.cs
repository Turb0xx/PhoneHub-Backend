using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhoneHub.Core.Entities;

namespace PhoneHub.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Nombre de la tabla en MySQL
            builder.ToTable("Users");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100); // Subimos a 100 por si los correos son largos

            builder.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(200); // Espacio suficiente para un hash de seguridad

            builder.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(20); // Para guardar "Admin" o "Seller"

            builder.Property(e => e.Telephone)
                .HasMaxLength(15) // Los números con código de país suelen ser más largos que 10
                .IsRequired(false);

            builder.Property(e => e.IsActive)
                .HasColumnType("bit") // El tipo estándar de MySQL para booleanos
                .IsRequired();
        }
    }
}