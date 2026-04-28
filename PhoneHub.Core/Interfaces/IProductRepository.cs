using PhoneHub.Core.Entities;

namespace PhoneHub.Core.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllAvailableAsync();
    }
}
