using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Enum;
using PhoneHub.Core.Interfaces;
using PhoneHub.Infrastructure.Data;
using PhoneHub.Infrastructure.Queries;

namespace PhoneHub.Infrastructure.Repositories
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        private readonly IDapperContext _dapper;

        public InvoiceRepository(PhoneHubContext context, IDapperContext dapper)
            : base(context)
        {
            _dapper = dapper;
        }

        public async Task<InvoiceDto?> GetByIdDapperAsync(int id)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DataBaseProvider.SqlServer => InvoiceQueries.GetByIdSqlServer,
                    DataBaseProvider.MySql => InvoiceQueries.GetByIdMySql,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryFirstOrDefaultAsync<InvoiceDto>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<InvoiceDto?> GetBySaleIdDapperAsync(int saleId)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DataBaseProvider.SqlServer => InvoiceQueries.GetBySaleIdSqlServer,
                    DataBaseProvider.MySql => InvoiceQueries.GetBySaleIdMySql,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryFirstOrDefaultAsync<InvoiceDto>(sql, new { SaleId = saleId });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ExistsBySaleIdAsync(int saleId)
        {
            return await _entities.AnyAsync(i => i.SaleId == saleId);
        }

        // RN-13: número de factura único y auto-generado
        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var count = await _entities.CountAsync();
            return $"PH-{DateTime.Now.Year}-{(count + 1):D6}";
        }
    }
}
