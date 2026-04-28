using Microsoft.EntityFrameworkCore;
using PhoneHub.Api.Filters;
using PhoneHub.Core.Interfaces;
using PhoneHub.Infrastructure.Data;
using PhoneHub.Infrastructure.Mappings;
using PhoneHub.Infrastructure.Repositories;
using PhoneHub.Services.Interfaces;
using PhoneHub.Services.Services;
using PhoneHub.Services.Validators;

namespace PhoneHub.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
            builder.Services.AddDbContext<PhoneHubContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Dapper
            builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            builder.Services.AddScoped<IDapperContext, DapperContext>();

            // UnitOfWork
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            builder.Services.AddTransient<IProductService, ProductService>();
            builder.Services.AddTransient<ISaleService, SaleService>();

            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling
                        = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

            builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);

            builder.Services.AddScoped<CrearProductoDtoValidator>();
            builder.Services.AddScoped<ActualizarProductoDtoValidator>();
            builder.Services.AddScoped<InventoryIngressDtoValidator>();
            builder.Services.AddScoped<SaleRequestDtoValidator>();

            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

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
