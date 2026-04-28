using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Interfaces;
using PhoneHub.Core.QueryFilters;
using PhoneHub.Services.Interfaces;

namespace PhoneHub.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(ProductQueryFilter? filters = null)
        {
            IEnumerable<Product> products;

            if (filters?.OnlyAvailable == true)
                products = await _productRepository.GetAllAvailableAsync();
            else
                products = await _productRepository.GetAll();

            if (filters != null)
            {
                if (filters.Brand != null)
                    products = products.Where(p => p.Brand.ToLower().Contains(filters.Brand.ToLower()));

                if (filters.Model != null)
                    products = products.Where(p => p.Model.ToLower().Contains(filters.Model.ToLower()));

                if (filters.MaxPrice != null)
                    products = products.Where(p => p.Price <= filters.MaxPrice);
            }

            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetById(id);
        }

        public async Task InsertProduct(Product product)
        {
            product.CreatedAt = DateTime.Now;
            await _productRepository.Add(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task DeleteProduct(int id)
        {
            await _productRepository.Delete(id);
            await _productRepository.SaveChangesAsync();
        }

        public async Task<bool> AddInventoryIngressAsync(InventoryIngressDto dto)
        {
            var product = await _productRepository.GetById(dto.ProductId);
            if (product == null)
                return false;

            product.Stock += dto.Quantity;
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
            return true;
        }
    }
}
