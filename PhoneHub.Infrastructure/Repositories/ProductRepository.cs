using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Enum;
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

        public async Task<IEnumerable<Product>> GetAllAvailableDapperAsync(int limit = 10)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DataBaseProvider.SqlServer => ProductQueries.GetAllAvailableSqlServer,
                    DataBaseProvider.MySql => ProductQueries.GetAllAvailableMySql,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<Product>(sql, new { Limit = limit });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
