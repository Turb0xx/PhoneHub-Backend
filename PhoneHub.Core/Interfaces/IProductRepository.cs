using PhoneHub.Core.Entities;

namespace PhoneHub.Core.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllDapperAsync();
        Task<Product?> GetByIdDapperAsync(int id);
        Task<IEnumerable<Product>> GetAllAvailableDapperAsync(int limit = 10);
    }
}
