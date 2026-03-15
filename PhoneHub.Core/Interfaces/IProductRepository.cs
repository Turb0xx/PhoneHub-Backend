using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;

namespace PhoneHub.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

        // Requerimiento: Registrar Nuevo Ingreso de Inventario
        Task<bool> AddInventoryIngressAsync(InventoryIngressDto dto);
    }
}
