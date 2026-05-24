using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PhoneHub.Api.Filters;
using PhoneHub.Core.CustomEntities;
using PhoneHub.Core.Interfaces;
using PhoneHub.Infrastructure.Data;
using PhoneHub.Infrastructure.Mappings;
using PhoneHub.Infrastructure.Repositories;
using PhoneHub.Services.Interfaces;
using PhoneHub.Services.Services;
using PhoneHub.Services.Validators;
using System.Text;

namespace PhoneHub.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Base de datos MySql
            var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
            builder.Services.AddDbContext<PhoneHubContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            #endregion

            #region Dapper
            builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            builder.Services.AddScoped<IDapperContext, DapperContext>();
            #endregion

            #region Repositorios y servicios
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddTransient<IProductService, ProductService>();
            builder.Services.AddTransient<ISaleService, SaleService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IPasswordService, PasswordService>();
            builder.Services.AddTransient<IInvoiceService, InvoiceService>();
            #endregion

            #region Configuración de contraseñas
            builder.Services.Configure<PasswordOptions>(
                builder.Configuration.GetSection("PasswordOptions"));
            #endregion

            #region Controllers y JSON
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
            #endregion

            #region AutoMapper
            builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);
            #endregion

            #region FluentValidation
            builder.Services.AddScoped<CrearProductoDtoValidator>();
            builder.Services.AddScoped<ActualizarProductoDtoValidator>();
            builder.Services.AddScoped<InventoryIngressDtoValidator>();
            builder.Services.AddScoped<SaleRequestDtoValidator>();
            builder.Services.AddScoped<CreateUserDtoValidator>();
            #endregion

            #region Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "PhoneHub API",
                    Version = "v1",
                    Description = "API REST para gestión y venta de celulares — PhoneHub",
                    Contact = new()
                    {
                        Name = "Equipo PhoneHub"
                    }
                });

                options.EnableAnnotations();

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                // Soporte JWT en Swagger
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Pegue SOLO el token JWT sin la palabra 'Bearer'. Ejemplo: eyJhbGci..."
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            #endregion

            #region JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Authentication:Issuer"],
                    ValidAudience = builder.Configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]!))
                };
            });
            #endregion

            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            #region Swagger UI
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PhoneHub API v1");
                    options.RoutePrefix = string.Empty;
                });
                app.MapOpenApi();
            }
            #endregion

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
