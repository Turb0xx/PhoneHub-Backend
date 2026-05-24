using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;

namespace PhoneHub.Core.Interfaces
{
    public interface ISaleRepository : IBaseRepository<Sale>
    {
        Task<IEnumerable<SaleResponseDto>> GetAllWithDetailsDapperAsync(int limit = 10);
        Task<SaleResponseDto?> GetByIdWithDetailsDapperAsync(int id);
        Task<bool> ExistsByProductIdAsync(int productId);
        Task<IEnumerable<SellerSummaryDto>> GetDailyReportAsync(DateTime date);
    }
}
