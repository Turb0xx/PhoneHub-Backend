using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.Entities;
using PhoneHub.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhoneHub.Infrastructure.Repositories
{
    public class ProductRepository
    {
        private readonly PhoneHubContext _context;

        public ProductRepository(PhoneHubContext context)
        {
            _context = context;
        }

        // Ahora se llama GetProducts para que tenga sentido con PhoneHub
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}