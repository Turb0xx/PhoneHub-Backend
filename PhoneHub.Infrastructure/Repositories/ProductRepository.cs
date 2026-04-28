using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Interfaces;
using PhoneHub.Infrastructure.Data;
using PhoneHub.Infrastructure.Queries;

namespace PhoneHub.Infrastructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly IDapperContext _dapper;

        public ProductRepository(PhoneHubContext context, IDapperContext dapper)
            : base(context)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Product>> GetAllAvailableAsync()
        {
            return await _entities
                .Where(p => p.Stock > 0)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAvailableDapperAsync()
        {
            try
            {
                return await _dapper.QueryAsync<Product>(ProductQueries.GetAllAvailable);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
