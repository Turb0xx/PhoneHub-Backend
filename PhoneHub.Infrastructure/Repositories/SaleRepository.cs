using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Interfaces;
using PhoneHub.Infrastructure.Data;

namespace PhoneHub.Infrastructure.Repositories
{
    public class SaleRepository : BaseRepository<Sale>, ISaleRepository
    {
        public SaleRepository(PhoneHubContext context) : base(context) { }

        public async Task<Sale?> GetByIdWithDetailsAsync(int id)
        {
            return await _entities
                .Include(s => s.Product)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Sale>> GetAllWithDetailsAsync()
        {
            return await _entities
                .Include(s => s.Product)
                .Include(s => s.User)
                .ToListAsync();
        }
    }
}
