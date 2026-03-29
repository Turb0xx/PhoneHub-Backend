using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Interfaces;
using PhoneHub.Services.Interfaces;

namespace PhoneHub.Services.Services
{
    public class SaleService : ISaleService
    {
        public readonly IBaseRepository<Sale> _saleRepository;
        public readonly IBaseRepository<Product> _productRepository;

        public SaleService(IBaseRepository<Sale> saleRepository,
            IBaseRepository<Product> productRepository)
        {
            _saleRepository = saleRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Sale>> GetAllSalesAsync()
        {
            return await _saleRepository.GetAll();
        }

        public async Task<Sale> GetSaleByIdAsync(int id)
        {
            return await _saleRepository.GetById(id);
        }

        public async Task<Sale> ProcessSaleAsync(SaleRequestDto dto)
        {
            var product = await _productRepository.GetById(dto.ProductId);
            if (product == null)
                throw new Exception("El producto no existe");

            if (product.Stock < dto.Quantity)
                throw new Exception($"Stock insuficiente. Stock disponible: {product.Stock} unidades");

            var sale = new Sale
            {
                ProductId = dto.ProductId,
                UserId = dto.UserId,
                Quantity = dto.Quantity,
                TotalAmount = product.Price * dto.Quantity,
                Date = DateTime.Now,
                IsActive = true
            };

            product.Stock -= dto.Quantity;
            await _productRepository.Update(product);
            await _saleRepository.Add(sale);

            return sale;
        }
    }
}
