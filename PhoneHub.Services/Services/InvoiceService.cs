using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Exceptions;
using PhoneHub.Core.Interfaces;
using PhoneHub.Services.Interfaces;
using System.Net;

namespace PhoneHub.Services.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<InvoiceDto> GenerateInvoiceAsync(int saleId)
        {
            // Verificar que la venta existe
            var sale = await _unitOfWork.SaleRepository.GetByIdWithDetailsDapperAsync(saleId);
            if (sale == null)
                throw new NotFoundException("Venta no encontrada.");

            // RN-12: una venta solo puede tener una factura
            var alreadyExists = await _unitOfWork.InvoiceRepository.ExistsBySaleIdAsync(saleId);
            if (alreadyExists)
                throw new BusinessException(
                    "Esta venta ya tiene una factura generada.",
                    HttpStatusCode.Conflict);

            // RN-13: número de factura único y auto-generado
            var invoiceNumber = await _unitOfWork.InvoiceRepository.GenerateInvoiceNumberAsync();

            var invoice = new Invoice
            {
                SaleId = saleId,
                InvoiceNumber = invoiceNumber,
                IssuedAt = DateTime.Now
            };

            await _unitOfWork.InvoiceRepository.Add(invoice);
            await _unitOfWork.SaveChangesAsync();

            // Retornar la factura completa con detalles via Dapper
            var invoiceDto = await _unitOfWork.InvoiceRepository.GetByIdDapperAsync(invoice.Id);
            return invoiceDto!;
        }

        public async Task<InvoiceDto?> GetByIdAsync(int id)
        {
            return await _unitOfWork.InvoiceRepository.GetByIdDapperAsync(id);
        }

        public async Task<InvoiceDto?> GetBySaleIdAsync(int saleId)
        {
            return await _unitOfWork.InvoiceRepository.GetBySaleIdDapperAsync(saleId);
        }
    }
}
