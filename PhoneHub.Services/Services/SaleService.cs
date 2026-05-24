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
    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllSalesAsync(SaleQueryFilter? filters = null)
        {
            filters ??= new SaleQueryFilter();

            var sales = await _unitOfWork.SaleRepository.GetAllWithDetailsDapperAsync(int.MaxValue);

            if (filters.UserId != null)
                sales = sales.Where(s => s.UserId == filters.UserId);

            if (filters.ProductId != null)
                sales = sales.Where(s => s.ProductId == filters.ProductId);

            var paged = PagedList<object>.Create(sales, filters.PageNumber, filters.PageSize);

            if (paged.Any())
            {
                return new ResponseData
                {
                    Messages = new[] { new Message { Type = TypeMessage.success.ToString(), Description = "Ventas recuperadas correctamente." } },
                    Pagination = paged,
                    StatusCode = HttpStatusCode.OK
                };
            }

            return new ResponseData
            {
                Messages = new[] { new Message { Type = TypeMessage.warning.ToString(), Description = "No se encontraron ventas con los filtros indicados." } },
                Pagination = paged,
                StatusCode = HttpStatusCode.NotFound
            };
        }

        public async Task<IEnumerable<SaleResponseDto>> GetAllSalesDapperAsync(int limit = 10)
        {
            return await _unitOfWork.SaleRepository.GetAllWithDetailsDapperAsync(limit);
        }

        public async Task<SaleResponseDto?> GetSaleByIdAsync(int id)
        {
            return await _unitOfWork.SaleRepository.GetByIdWithDetailsDapperAsync(id);
        }

        public async Task<Sale> ProcessSaleAsync(SaleRequestDto dto)
        {
            var product = await _unitOfWork.ProductRepository.GetById(dto.ProductId);
            if (product == null)
                throw new NotFoundException("El producto no existe.");

            var user = await _unitOfWork.UserRepository.GetById(dto.UserId);
            if (user == null)
                throw new NotFoundException("El usuario no existe.");

            // RN-04: usuario inactivo no puede realizar ventas
            if (!user.IsActive)
                throw new BusinessException("Usuario inactivo, contacte al administrador.", System.Net.HttpStatusCode.Forbidden);

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

        public async Task<CashCloseReportDto> GetDailyCloseReportAsync(DateTime date)
        {
            // RN-10: solo se consideran ventas con IsActive = true (la query filtra en DB)
            var sellers = await _unitOfWork.SaleRepository.GetDailyReportAsync(date);
            var sellerList = sellers.ToList();

            // CU-07 Flujo A: si no hay ventas, devolver reporte con totales en cero
            return new CashCloseReportDto
            {
                Date = date.ToString("yyyy-MM-dd"),
                TotalSales = sellerList.Sum(s => s.SalesCount),
                TotalAmount = sellerList.Sum(s => s.SubTotal),
                Sellers = sellerList
            };
        }

        public async Task AnnulSaleAsync(int id)
        {
            var sale = await _unitOfWork.SaleRepository.GetById(id);
            if (sale == null)
                throw new NotFoundException("Venta no encontrada.");

            // RN-05: baja lógica — no se elimina físicamente
            sale.IsActive = false;
            _unitOfWork.SaleRepository.Update(sale);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
