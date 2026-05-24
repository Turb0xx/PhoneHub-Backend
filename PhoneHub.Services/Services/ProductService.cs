using PhoneHub.Core.CustomEntities;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Enum;
using PhoneHub.Core.Exceptions;
using PhoneHub.Core.Interfaces;
using PhoneHub.Core.QueryFilters;
using PhoneHub.Services.Interfaces;
using System.Net;

namespace PhoneHub.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllProductsAsync(ProductQueryFilter? filters = null)
        {
            filters ??= new ProductQueryFilter();

            var products = await _unitOfWork.ProductRepository.GetAllDapperAsync();

            if (filters.OnlyAvailable == true)
                products = products.Where(p => p.Stock > 0);

            if (filters.Brand != null)
                products = products.Where(p => p.Brand.ToLower().Contains(filters.Brand.ToLower()));

            if (filters.Model != null)
                products = products.Where(p => p.Model.ToLower().Contains(filters.Model.ToLower()));

            if (filters.MaxPrice != null)
                products = products.Where(p => p.Price <= filters.MaxPrice);

            var paged = PagedList<object>.Create(products, filters.PageNumber, filters.PageSize);

            if (paged.Any())
            {
                return new ResponseData
                {
                    Messages = new[] { new Message { Type = TypeMessage.success.ToString(), Description = "Productos recuperados correctamente." } },
                    Pagination = paged,
                    StatusCode = HttpStatusCode.OK
                };
            }

            return new ResponseData
            {
                Messages = new[] { new Message { Type = TypeMessage.warning.ToString(), Description = "No se encontraron productos con los filtros indicados." } },
                Pagination = paged,
                StatusCode = HttpStatusCode.NotFound
            };
        }

        public async Task<IEnumerable<Product>> GetAvailableProductsDapperAsync(int limit = 10)
        {
            return await _unitOfWork.ProductRepository.GetAllAvailableDapperAsync(limit);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.ProductRepository.GetByIdDapperAsync(id);
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
            var hasSales = await _unitOfWork.SaleRepository.ExistsByProductIdAsync(id);
            if (hasSales)
                throw new BusinessException("No se puede eliminar el producto porque tiene ventas asociadas.");

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
