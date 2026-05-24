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

        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}