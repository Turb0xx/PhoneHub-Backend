using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;

namespace PhoneHub.Core.Interfaces
{
    public interface ISaleRepository
    {
        Task<IEnumerable<Sale>> GetSalesAsync();
        Task<Sale?> GetSaleByIdAsync(int id);

        // Requerimiento: Procesar una Venta
        Task<Sale> ProcessSaleAsync(SaleRequestDto dto);
    }
}
