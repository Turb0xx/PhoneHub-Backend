using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;

namespace PhoneHub.Core.Interfaces
{
    public interface IInvoiceRepository : IBaseRepository<Invoice>
    {
        Task<InvoiceDto?> GetByIdDapperAsync(int id);
        Task<InvoiceDto?> GetBySaleIdDapperAsync(int saleId);
        Task<bool> ExistsBySaleIdAsync(int saleId);
        Task<string> GenerateInvoiceNumberAsync();
    }
}
