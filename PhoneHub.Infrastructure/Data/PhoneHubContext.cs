using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.Entities; 
using System.Reflection;

namespace PhoneHub.Infrastructure.Data
{
    public partial class PhoneHubContext : DbContext
    {
        public PhoneHubContext()
        {
        }

        public PhoneHubContext(DbContextOptions<PhoneHubContext> options)
            : base(options)
        {
        }

        // Definimos las tablas con nombres lógicos para tu sistema
        public virtual DbSet<Sale> Sales { get; set; } // Antes era Comments
        public virtual DbSet<Product> Products { get; set; } // Antes era Posts
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Esta línea es clave: busca automáticamente los archivos en la carpeta "Configurations"
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}