using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Interfaces;
using PhoneHub.Services.Interfaces;

namespace PhoneHub.Services.Services
{
    public class ProductService : IProductService
    {
        public readonly IBaseRepository<Product> _productRepository;

        public ProductService(IBaseRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAll();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetById(id);
        }

        public async Task InsertProduct(Product product)
        {
            product.CreatedAt = DateTime.Now;
            await _productRepository.Add(product);
        }

        public async Task UpdateProduct(Product product)
        {
            await _productRepository.Update(product);
        }

        public async Task DeleteProduct(int id)
        {
            await _productRepository.Delete(id);
        }

        public async Task<bool> AddInventoryIngressAsync(InventoryIngressDto dto)
        {
            var product = await _productRepository.GetById(dto.ProductId);
            if (product == null)
                return false;

            product.Stock += dto.Quantity;
            await _productRepository.Update(product);
            return true;
        }
    }
}
