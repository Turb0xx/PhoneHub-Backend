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
        private readonly IUnitOfWork _unitOfWork;

        public SaleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Sale>> GetAllSalesAsync()
        {
            return await _unitOfWork.SaleRepository.GetAllWithDetailsAsync();
        }

        public async Task<IEnumerable<Sale>> GetAllSalesDapperAsync()
        {
            return await _unitOfWork.SaleRepository.GetAllWithDetailsDapperAsync();
        }

        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            return await _unitOfWork.SaleRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<Sale> ProcessSaleAsync(SaleRequestDto dto)
        {
            var product = await _unitOfWork.ProductRepository.GetById(dto.ProductId);
            if (product == null)
                throw new NotFoundException("El producto no existe.");

            var user = await _unitOfWork.UserRepository.GetById(dto.UserId);
            if (user == null)
                throw new NotFoundException("El usuario no existe.");

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
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaleRepository.Add(sale);
            await _unitOfWork.SaveChangesAsync();

            sale.Product = product;
            sale.User = user;

            return sale;
        }
    }
}
