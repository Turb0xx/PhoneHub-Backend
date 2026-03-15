using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Interfaces;
using PhoneHub.Infrastructure.Data;

namespace PhoneHub.Infrastructure.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly PhoneHubContext _context;

        public SaleRepository(PhoneHubContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sale>> GetSalesAsync()
        {
            return await _context.Sales
                .Include(s => s.Product)
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.Product)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // Requerimiento: Procesar una Venta
        // 1. Verifica que el producto exista
        // 2. Verifica que haya stock suficiente
        // 3. Crea la venta y calcula el total automáticamente
        // 4. Descuenta el stock del producto
        public async Task<Sale> ProcessSaleAsync(SaleRequestDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId)
                ?? throw new Exception("Producto no encontrado");

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
            _context.Products.Update(product);
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return sale;
        }
    }
}
