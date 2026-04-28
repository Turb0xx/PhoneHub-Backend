using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Interfaces;
using PhoneHub.Infrastructure.Data;

namespace PhoneHub.Infrastructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(PhoneHubContext context) : base(context) { }

        public async Task<IEnumerable<Product>> GetAllAvailableAsync()
        {
            return await _entities
                .Where(p => p.Stock > 0)
                .ToListAsync();
        }
    }
}
