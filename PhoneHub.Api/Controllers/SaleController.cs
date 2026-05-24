using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneHub.Api.Responses;
using PhoneHub.Core.CustomEntities;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Enum;
using PhoneHub.Core.QueryFilters;
using PhoneHub.Services.Interfaces;
using PhoneHub.Services.Validators;
using System.Net;

namespace PhoneHub.Api.Controllers
{
    [Authorize]
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

        /// <summary>
        /// Obtiene la lista paginada de ventas activas con opción de filtrar por vendedor o producto.
        /// </summary>
        /// <param name="filters">Filtros de paginación, UserId y ProductId.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<SaleResponseDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SaleQueryFilter? filters)
        {
            var result = await _saleService.GetAllSalesAsync(filters);
            var salesDto = result.Pagination.Cast<SaleResponseDto>();

            var pagination = new Pagination
            {
                TotalCount = result.Pagination.TotalCount,
                PageSize = result.Pagination.PageSize,
                CurrentPage = result.Pagination.CurrentPage,
                TotalPages = result.Pagination.TotalPages,
                HasNextPage = result.Pagination.HasNextPage,
                HasPreviousPage = result.Pagination.HasPreviousPage
            };

            var response = new ApiResponse<IEnumerable<SaleResponseDto>>(salesDto)
            {
                Pagination = pagination,
                Messages = result.Messages
            };

            return StatusCode((int)result.StatusCode, response);
        }

        /// <summary>
        /// Obtiene el detalle de una venta por su ID (CU-05 — Comprobante de Venta).
        /// </summary>
        /// <param name="id">ID de la venta.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<SaleResponseDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
                return NotFound("Venta no encontrada.");

            var response = new ApiResponse<SaleResponseDto>(sale);
            return Ok(response);
        }

        /// <summary>
        /// Genera el reporte de cierre de caja diario (CU-07).
        /// RN-10: solo considera ventas con IsActive = true. Solo accesible para Admin.
        /// </summary>
        /// <param name="date">Fecha del cierre. Por defecto: hoy.</param>
        [Authorize(Roles = nameof(RoleType.Admin))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<CashCloseReportDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [HttpGet("reporte/cierre-diario")]
        public async Task<IActionResult> GetDailyCloseReport([FromQuery] DateTime? date = null)
        {
            var reportDate = date ?? DateTime.Today;
            var report = await _saleService.GetDailyCloseReportAsync(reportDate);
            var response = new ApiResponse<CashCloseReportDto>(report);
            return Ok(response);
        }

        /// <summary>
        /// Procesa una nueva venta (CU-03).
        /// RN-01: verifica stock. RN-02: calcula total automáticamente. RN-03: operación atómica.
        /// </summary>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<SaleResponseDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
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

        /// <summary>
        /// Anula una venta (RN-05). No se elimina físicamente: cambia IsActive a false.
        /// Solo accesible para Admin.
        /// </summary>
        /// <param name="id">ID de la venta a anular.</param>
        [Authorize(Roles = nameof(RoleType.Admin))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Annul(int id)
        {
            await _saleService.AnnulSaleAsync(id);
            var response = new ApiResponse<string>($"Venta con ID {id} anulada correctamente. El registro permanece en el sistema con estado inactivo (RN-05).");
            return Ok(response);
        }

        [HttpGet("dapper")]
        public async Task<IActionResult> GetAllDapper([FromQuery] int limit = 10)
        {
            var sales = await _saleService.GetAllSalesDapperAsync(limit);
            var response = new ApiResponse<IEnumerable<SaleResponseDto>>(sales);
            return Ok(response);
        }
    }
}
