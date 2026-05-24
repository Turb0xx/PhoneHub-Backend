using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneHub.Api.Responses;
using PhoneHub.Core.DTOs;
using PhoneHub.Services.Interfaces;
using System.Net;

namespace PhoneHub.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Genera una factura/comprobante para una venta procesada (CU-05).
        /// RN-12: Una venta solo puede tener una factura. RN-13: Número único auto-generado.
        /// </summary>
        /// <param name="saleId">ID de la venta a facturar.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<InvoiceDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [HttpPost("generate/{saleId}")]
        public async Task<IActionResult> Generate(int saleId)
        {
            var invoice = await _invoiceService.GenerateInvoiceAsync(saleId);
            var response = new ApiResponse<InvoiceDto>(invoice);
            return Ok(response);
        }

        /// <summary>
        /// Obtiene el comprobante de una venta por ID de factura.
        /// </summary>
        /// <param name="id">ID de la factura.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<InvoiceDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null)
                return NotFound("Factura no encontrada.");

            var response = new ApiResponse<InvoiceDto>(invoice);
            return Ok(response);
        }

        /// <summary>
        /// Obtiene el comprobante de una venta por ID de venta (CU-05).
        /// </summary>
        /// <param name="saleId">ID de la venta.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<InvoiceDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [HttpGet("sale/{saleId}")]
        public async Task<IActionResult> GetBySaleId(int saleId)
        {
            var invoice = await _invoiceService.GetBySaleIdAsync(saleId);
            if (invoice == null)
                return NotFound("No existe una factura para esta venta.");

            var response = new ApiResponse<InvoiceDto>(invoice);
            return Ok(response);
        }
    }
}
