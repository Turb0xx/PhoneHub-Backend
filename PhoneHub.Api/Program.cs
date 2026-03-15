using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.Interfaces;
using PhoneHub.Infrastructure.Data;
using PhoneHub.Infrastructure.Repositories;

namespace PhoneHub.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configurar la BD MySql
            var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
            builder.Services.AddDbContext<PhoneHubContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    b => b.MigrationsAssembly("PhoneHub.Infrastructure")));
            #endregion

            #region Registro de Repositorios
            builder.Services.AddTransient<IProductRepository, ProductRepository>();
            builder.Services.AddTransient<ISaleRepository, SaleRepository>();
            #endregion

            // AddNewtonsoftJson evita errores de ciclos infinitos por las relaciones Sale->Product->Sale
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            builder.Services.AddOpenApi();

            var app = builder.Build();

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
