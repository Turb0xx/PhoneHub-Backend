using PhoneHub.Core.Entities;

namespace PhoneHub.Core.Interfaces
{
    public interface ISaleRepository : IBaseRepository<Sale>
    {
        Task<Sale?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Sale>> GetAllWithDetailsAsync();
    }
}
