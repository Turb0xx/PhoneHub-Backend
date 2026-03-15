using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Interfaces;
using PhoneHub.Infrastructure.Data;

namespace PhoneHub.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly PhoneHubContext _context;

        public ProductRepository(PhoneHubContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddProductAsync(Product product)
        {
            product.CreatedAt = DateTime.Now;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        // Requerimiento: Registrar Nuevo Ingreso de Inventario
        // Suma las unidades recibidas al stock actual del producto
        public async Task<bool> AddInventoryIngressAsync(InventoryIngressDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null) return false;

            product.Stock += dto.Quantity;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
