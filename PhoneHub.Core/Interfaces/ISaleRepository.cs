using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;

namespace PhoneHub.Core.Interfaces
{
    public interface ISaleRepository : IBaseRepository<Sale>
    {
        Task<Sale?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Sale>> GetAllWithDetailsAsync();
        Task<IEnumerable<SaleResponseDto>> GetAllWithDetailsDapperAsync(int limit = 10);
        Task<bool> ExistsByProductIdAsync(int productId);
    }
}
