using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Enum;
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

        public async Task<IEnumerable<SaleResponseDto>> GetAllWithDetailsDapperAsync(int limit = 10)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DataBaseProvider.SqlServer => SaleQueries.GetAllWithDetailsSqlServer,
                    DataBaseProvider.MySql => SaleQueries.GetAllWithDetailsMySql,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<SaleResponseDto>(sql, new { Limit = limit });
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
