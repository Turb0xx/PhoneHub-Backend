using Microsoft.EntityFrameworkCore;
using PhoneHub.Infrastructure.Data; // Actualizado de SocialMedia a PhoneHub

namespace PhoneHub.Api // Actualizado el namespace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Configurar la BD MySql
            var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");

            builder.Services.AddDbContext<PhoneHubContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    // ESTA ES LA LÍNEA CLAVE:
                    b => b.MigrationsAssembly("PhoneHub.Infrastructure")));
            #endregion

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}