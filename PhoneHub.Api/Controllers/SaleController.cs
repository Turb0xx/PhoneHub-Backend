using Microsoft.AspNetCore.Mvc;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Interfaces;

namespace PhoneHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleRepository _saleRepository;

        public SaleController(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        // GET: api/sale
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sales = await _saleRepository.GetSalesAsync();
            return Ok(sales);
        }

        // GET: api/sale/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sale = await _saleRepository.GetSaleByIdAsync(id);
            if (sale == null)
                return NotFound(new { message = "Venta no encontrada" });
            return Ok(sale);
        }

        // POST: api/sale/procesar
        // ✅ Requerimiento: Procesar una Venta
        // Body ejemplo: { "productId": 1, "userId": 2, "quantity": 3 }
        [HttpPost("procesar")]
        public async Task<IActionResult> ProcessSale(SaleRequestDto dto)
        {
            try
            {
                var sale = await _saleRepository.ProcessSaleAsync(dto);
                return Created($"api/sale/{sale.Id}", new
                {
                    message = "Venta procesada exitosamente",
                    saleId = sale.Id,
                    totalAmount = sale.TotalAmount,
                    date = sale.Date
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
