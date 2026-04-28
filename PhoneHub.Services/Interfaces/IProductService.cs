using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.QueryFilters;

namespace PhoneHub.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(ProductQueryFilter? filters = null);
        Task<IEnumerable<Product>> GetAvailableProductsDapperAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task InsertProduct(Product product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(int id);
        Task<bool> AddInventoryIngressAsync(InventoryIngressDto dto);
    }
}
