using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Exceptions;
using PhoneHub.Core.Interfaces;
using PhoneHub.Services.Interfaces;
using System.Net;

namespace PhoneHub.Services.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IProductRepository _productRepository;
        private readonly IBaseRepository<User> _userRepository;

        public SaleService(
            ISaleRepository saleRepository,
            IProductRepository productRepository,
            IBaseRepository<User> userRepository)
        {
            _saleRepository = saleRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Sale>> GetAllSalesAsync()
        {
            return await _saleRepository.GetAllWithDetailsAsync();
        }

        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            return await _saleRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<Sale> ProcessSaleAsync(SaleRequestDto dto)
        {
            var product = await _productRepository.GetById(dto.ProductId);
            if (product == null)
                throw new BusinessException("El producto no existe.", HttpStatusCode.NotFound);

            var user = await _userRepository.GetById(dto.UserId);
            if (user == null)
                throw new BusinessException("El usuario no existe.", HttpStatusCode.NotFound);

            if (product.Stock < dto.Quantity)
                throw new BusinessException(
                    $"Stock insuficiente. Disponible: {product.Stock} unidades.",
                    HttpStatusCode.BadRequest);

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
            _productRepository.Update(product);
            await _saleRepository.Add(sale);
            await _saleRepository.SaveChangesAsync(); // guarda update + insert en una sola transacción

            // asignar nav props para el mapeo del comprobante
            sale.Product = product;
            sale.User = user;

            return sale;
        }
    }
}
