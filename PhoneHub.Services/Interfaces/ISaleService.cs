using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.QueryFilters;

namespace PhoneHub.Services.Interfaces
{
    public interface ISaleService
    {
        Task<IEnumerable<Sale>> GetAllSalesAsync(SaleQueryFilter? filters = null);
        Task<IEnumerable<SaleResponseDto>> GetAllSalesDapperAsync(int limit = 10);
        Task<Sale?> GetSaleByIdAsync(int id);
        Task<Sale> ProcessSaleAsync(SaleRequestDto dto);
    }
}
