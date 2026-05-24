using PhoneHub.Core.DTOs;

namespace PhoneHub.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> GenerateInvoiceAsync(int saleId);
        Task<InvoiceDto?> GetByIdAsync(int id);
        Task<InvoiceDto?> GetBySaleIdAsync(int saleId);
    }
}
