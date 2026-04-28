using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PhoneHub.Api.Responses;
using PhoneHub.Core.DTOs;
using PhoneHub.Services.Interfaces;
using PhoneHub.Services.Validators;

namespace PhoneHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly IMapper _mapper;
        private readonly SaleRequestDtoValidator _saleValidator;

        public SaleController(
            ISaleService saleService,
            IMapper mapper,
            SaleRequestDtoValidator saleValidator)
        {
            _saleService = saleService;
            _mapper = mapper;
            _saleValidator = saleValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sales = await _saleService.GetAllSalesAsync();
            var salesDto = _mapper.Map<IEnumerable<SaleResponseDto>>(sales);
            var response = new ApiResponse<IEnumerable<SaleResponseDto>>(salesDto);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
                return NotFound("Venta no encontrada.");

            var saleDto = _mapper.Map<SaleResponseDto>(sale);
            var response = new ApiResponse<SaleResponseDto>(saleDto);
            return Ok(response);
        }

        [HttpPost("procesar")]
        public async Task<IActionResult> ProcessSale(SaleRequestDto dto)
        {
            var validationResult = await _saleValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var sale = await _saleService.ProcessSaleAsync(dto);
            var saleDto = _mapper.Map<SaleResponseDto>(sale);
            var response = new ApiResponse<SaleResponseDto>(saleDto);
            return Ok(response);
        }
    }
}
