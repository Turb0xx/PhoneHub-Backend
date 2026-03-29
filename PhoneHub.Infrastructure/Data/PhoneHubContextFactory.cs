using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PhoneHub.Infrastructure.Data
{
    public class PhoneHubContextFactory : IDesignTimeDbContextFactory<PhoneHubContext>
    {
        public PhoneHubContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PhoneHubContext>();
            optionsBuilder.UseMySql(
                "Server=localhost;Port=3306;Database=DbPhoneHub;Uid=root;Pwd=root;",
                new MySqlServerVersion(new Version(8, 0, 0)),
                b => b.MigrationsAssembly("PhoneHub.Infrastructure"));

            return new PhoneHubContext(optionsBuilder.Options);
        }
    }
}
