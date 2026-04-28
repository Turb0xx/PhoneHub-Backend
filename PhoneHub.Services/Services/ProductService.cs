using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Exceptions;
using PhoneHub.Core.Interfaces;
using PhoneHub.Core.QueryFilters;
using PhoneHub.Services.Interfaces;

namespace PhoneHub.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(ProductQueryFilter? filters = null)
        {
            IEnumerable<Product> products;

            if (filters?.OnlyAvailable == true)
                products = await _unitOfWork.ProductRepository.GetAllAvailableAsync();
            else
                products = await _unitOfWork.ProductRepository.GetAll();

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

        public async Task<IEnumerable<Product>> GetAvailableProductsDapperAsync()
        {
            return await _unitOfWork.ProductRepository.GetAllAvailableDapperAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.ProductRepository.GetById(id);
        }

        public async Task InsertProduct(Product product)
        {
            product.CreatedAt = DateTime.Now;
            await _unitOfWork.ProductRepository.Add(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProduct(int id)
        {
            await _unitOfWork.ProductRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> AddInventoryIngressAsync(InventoryIngressDto dto)
        {
            var product = await _unitOfWork.ProductRepository.GetById(dto.ProductId);
            if (product == null)
                throw new NotFoundException("El producto no existe.");

            product.Stock += dto.Quantity;
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
