using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.Entities;

namespace PhoneHub.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(PhoneHubContext context)
        {
            await SeedUsersAsync(context);
            await SeedProductsAsync(context);
            await SeedSalesAsync(context);
        }

        // ─────────────────────────────────────────────
        // USERS
        // ─────────────────────────────────────────────
        private static async Task SeedUsersAsync(PhoneHubContext context)
        {
            if (await context.Users.AnyAsync())
                return;

            var users = new List<User>
            {
                new User
                {
                    FirstName  = "Carlos",
                    LastName   = "Mamani",
                    Email      = "carlos@phonehub.com",
                    Password   = "password123",
                    Role       = "Admin",
                    Telephone  = "77712345",
                    IsActive   = true
                },
                new User
                {
                    FirstName  = "Ana",
                    LastName   = "Flores",
                    Email      = "ana@phonehub.com",
                    Password   = "password123",
                    Role       = "Cliente",
                    Telephone  = "76698765",
                    IsActive   = true
                },
                new User
                {
                    FirstName  = "Luis",
                    LastName   = "Quispe",
                    Email      = "luis@phonehub.com",
                    Password   = "password123",
                    Role       = "Cliente",
                    Telephone  = "71134567",
                    IsActive   = true
                },
                new User
                {
                    FirstName  = "María",
                    LastName   = "Torrez",
                    Email      = "maria@phonehub.com",
                    Password   = "password123",
                    Role       = "Cliente",
                    Telephone  = "79923456",
                    IsActive   = true
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }

        // ─────────────────────────────────────────────
        // PRODUCTS
        // ─────────────────────────────────────────────
        private static async Task SeedProductsAsync(PhoneHubContext context)
        {
            if (await context.Products.AnyAsync())
                return;

            var now = DateTime.Now;

            var products = new List<Product>
            {
                new Product
                {
                    Brand       = "Samsung",
                    Model       = "Galaxy A55",
                    Description = "128GB AMOLED 6.6\" cámara 50MP",
                    Price       = 1850m,
                    Stock       = 15,
                    CreatedAt   = now
                },
                new Product
                {
                    Brand       = "Samsung",
                    Model       = "Galaxy S24",
                    Description = "256GB IA integrada flagship",
                    Price       = 4200m,
                    Stock       = 8,
                    CreatedAt   = now
                },
                new Product
                {
                    Brand       = "Xiaomi",
                    Model       = "Redmi Note 13",
                    Description = "128GB batería 5000mAh cámara 108MP",
                    Price       = 1200m,
                    Stock       = 20,
                    CreatedAt   = now
                },
                new Product
                {
                    Brand       = "Xiaomi",
                    Model       = "POCO X6",
                    Description = "256GB Snapdragon 7s Gen 2 carga 67W",
                    Price       = 1650m,
                    Stock       = 12,
                    CreatedAt   = now
                },
                new Product
                {
                    Brand       = "Apple",
                    Model       = "iPhone 15",
                    Description = "128GB chip A16 Dynamic Island",
                    Price       = 5800m,
                    Stock       = 5,
                    CreatedAt   = now
                },
                new Product
                {
                    Brand       = "Apple",
                    Model       = "iPhone 14",
                    Description = "128GB chip A15 MagSafe",
                    Price       = 4500m,
                    Stock       = 3,
                    CreatedAt   = now
                },
                new Product
                {
                    Brand       = "Motorola",
                    Model       = "Moto G84",
                    Description = "256GB pOLED 5G",
                    Price       = 1400m,
                    Stock       = 18,
                    CreatedAt   = now
                },
                new Product
                {
                    Brand       = "Motorola",
                    Model       = "Edge 40",
                    Description = "256GB pantalla curva 144Hz",
                    Price       = 2100m,
                    Stock       = 7,
                    CreatedAt   = now
                },
                new Product
                {
                    Brand       = "Huawei",
                    Model       = "Nova 11",
                    Description = "256GB cámara frontal 60MP",
                    Price       = 1750m,
                    Stock       = 0,   // sin stock — prueba onlyAvailable=true
                    CreatedAt   = now
                },
                new Product
                {
                    Brand       = "Samsung",
                    Model       = "Galaxy A14",
                    Description = "64GB económico entrada de gama",
                    Price       = 850m,
                    Stock       = 0,   // sin stock — prueba onlyAvailable=true
                    CreatedAt   = now
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        // ─────────────────────────────────────────────
        // SALES
        // ─────────────────────────────────────────────
        private static async Task SeedSalesAsync(PhoneHubContext context)
        {
            if (await context.Sales.AnyAsync())
                return;

            // Recuperar por email/modelo para no depender del orden de IDs
            var ana   = await context.Users.FirstAsync(u => u.Email == "ana@phonehub.com");
            var luis  = await context.Users.FirstAsync(u => u.Email == "luis@phonehub.com");
            var maria = await context.Users.FirstAsync(u => u.Email == "maria@phonehub.com");

            var galaxyA55    = await context.Products.FirstAsync(p => p.Model == "Galaxy A55");
            var redmiNote13  = await context.Products.FirstAsync(p => p.Model == "Redmi Note 13");
            var iphone15     = await context.Products.FirstAsync(p => p.Model == "iPhone 15");
            var motoG84      = await context.Products.FirstAsync(p => p.Model == "Moto G84");
            var galaxyS24    = await context.Products.FirstAsync(p => p.Model == "Galaxy S24");

            var sales = new List<Sale>
            {
                new Sale
                {
                    UserId      = ana.Id,
                    ProductId   = galaxyA55.Id,
                    Quantity    = 1,
                    TotalAmount = 1850m,
                    Date        = DateTime.Now.AddDays(-30),
                    IsActive    = true
                },
                new Sale
                {
                    UserId      = luis.Id,
                    ProductId   = redmiNote13.Id,
                    Quantity    = 2,
                    TotalAmount = 2400m,
                    Date        = DateTime.Now.AddDays(-22),
                    IsActive    = true
                },
                new Sale
                {
                    UserId      = maria.Id,
                    ProductId   = iphone15.Id,
                    Quantity    = 1,
                    TotalAmount = 5800m,
                    Date        = DateTime.Now.AddDays(-15),
                    IsActive    = true
                },
                new Sale
                {
                    UserId      = ana.Id,
                    ProductId   = motoG84.Id,
                    Quantity    = 1,
                    TotalAmount = 1400m,
                    Date        = DateTime.Now.AddDays(-8),
                    IsActive    = true
                },
                new Sale
                {
                    UserId      = luis.Id,
                    ProductId   = galaxyS24.Id,
                    Quantity    = 1,
                    TotalAmount = 4200m,
                    Date        = DateTime.Now.AddDays(-3),
                    IsActive    = true
                }
            };

            await context.Sales.AddRangeAsync(sales);
            await context.SaveChangesAsync();
        }
    }
}
