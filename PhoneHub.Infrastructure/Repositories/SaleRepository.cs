using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Interfaces;
using PhoneHub.Infrastructure.Data;
using PhoneHub.Infrastructure.Queries;

namespace PhoneHub.Infrastructure.Repositories
{
    public class SaleRepository : BaseRepository<Sale>, ISaleRepository
    {
        private readonly IDapperContext _dapper;

        public SaleRepository(PhoneHubContext context, IDapperContext dapper)
            : base(context)
        {
            _dapper = dapper;
        }

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

        public async Task<IEnumerable<Sale>> GetAllWithDetailsDapperAsync()
        {
            try
            {
                return await _dapper.QueryAsync<Sale>(SaleQueries.GetAllWithDetails);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ExistsByProductIdAsync(int productId)
        {
            return await _entities.AnyAsync(s => s.ProductId == productId);
        }
    }
}
