using Microsoft.AspNetCore.Mvc;
using PhoneHub.Api.Responses;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Services.Interfaces;
using PhoneHub.Services.Validators;

namespace PhoneHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly SaleRequestDtoValidator _saleValidator;

        public SaleController(
            ISaleService saleService,
            SaleRequestDtoValidator saleValidator)
        {
            _saleService = saleService;
            _saleValidator = saleValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sales = await _saleService.GetAllSalesAsync();
            var response = new ApiResponse<IEnumerable<Sale>>(sales);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
                return NotFound("Venta no encontrada.");

            var response = new ApiResponse<Sale>(sale);
            return Ok(response);
        }

        [HttpPost("procesar")]
        public async Task<IActionResult> ProcessSale(SaleRequestDto dto)
        {
            var validationResult = await _saleValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    message = "Error de validación",
                    errors = validationResult.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            try
            {
                var sale = await _saleService.ProcessSaleAsync(dto);
                var response = new ApiResponse<Sale>(sale);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
