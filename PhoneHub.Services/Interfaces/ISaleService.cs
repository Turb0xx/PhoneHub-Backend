using PhoneHub.Core.CustomEntities;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.QueryFilters;

namespace PhoneHub.Services.Interfaces
{
    public interface ISaleService
    {
        Task<ResponseData> GetAllSalesAsync(SaleQueryFilter? filters = null);
        Task<IEnumerable<SaleResponseDto>> GetAllSalesDapperAsync(int limit = 10);
        Task<SaleResponseDto?> GetSaleByIdAsync(int id);
        Task<Sale> ProcessSaleAsync(SaleRequestDto dto);
        Task<CashCloseReportDto> GetDailyCloseReportAsync(DateTime date);
        Task AnnulSaleAsync(int id);
    }
}
