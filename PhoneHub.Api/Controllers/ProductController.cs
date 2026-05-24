using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneHub.Api.Responses;
using PhoneHub.Core.CustomEntities;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.QueryFilters;
using PhoneHub.Services.Interfaces;
using PhoneHub.Services.Validators;
using System.Net;

namespace PhoneHub.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly CrearProductoDtoValidator _crearValidator;
        private readonly ActualizarProductoDtoValidator _actualizarValidator;
        private readonly InventoryIngressDtoValidator _inventoryValidator;

        public ProductController(
            IProductService productService,
            IMapper mapper,
            CrearProductoDtoValidator crearValidator,
            ActualizarProductoDtoValidator actualizarValidator,
            InventoryIngressDtoValidator inventoryValidator)
        {
            _productService = productService;
            _mapper = mapper;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
            _inventoryValidator = inventoryValidator;
        }

        /// <summary>
        /// Obtiene la lista paginada de productos con filtros opcionales (CU-04).
        /// </summary>
        /// <param name="filters">Filtros: Brand, Model, MaxPrice, OnlyAvailable, PageNumber, PageSize.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<ProductDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductQueryFilter? filters)
        {
            var result = await _productService.GetAllProductsAsync(filters);
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(result.Pagination);

            var pagination = new Pagination
            {
                TotalCount = result.Pagination.TotalCount,
                PageSize = result.Pagination.PageSize,
                CurrentPage = result.Pagination.CurrentPage,
                TotalPages = result.Pagination.TotalPages,
                HasNextPage = result.Pagination.HasNextPage,
                HasPreviousPage = result.Pagination.HasPreviousPage
            };

            var response = new ApiResponse<IEnumerable<ProductDto>>(productsDto)
            {
                Pagination = pagination,
                Messages = result.Messages
            };

            return StatusCode((int)result.StatusCode, response);
        }

        /// <summary>
        /// Obtiene el detalle de un producto por su ID (CU-04).
        /// </summary>
        /// <param name="id">ID del producto.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<ProductDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Producto no encontrado.");

            var productDto = _mapper.Map<ProductDto>(product);
            var response = new ApiResponse<ProductDto>(productDto);
            return Ok(response);
        }

        /// <summary>
        /// Crea un nuevo producto en el inventario.
        /// RN-07: el precio debe ser mayor a cero.
        /// </summary>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<ProductDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [HttpPost]
        public async Task<IActionResult> Insert(ProductDto productDto)
        {
            var validationResult = await _crearValidator.ValidateAsync(productDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var product = _mapper.Map<Product>(productDto);
            await _productService.InsertProduct(product);
            var createdDto = _mapper.Map<ProductDto>(product);
            var response = new ApiResponse<ProductDto>(createdDto);
            return Ok(response);
        }

        /// <summary>
        /// Actualiza los datos de un producto existente.
        /// </summary>
        /// <param name="id">ID del producto a actualizar.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<ProductDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto productDto)
        {
            if (id != productDto.Id)
                return BadRequest("El ID del producto no coincide.");

            var validationResult = await _actualizarValidator.ValidateAsync(productDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Producto no encontrado.");

            _mapper.Map(productDto, product);
            await _productService.UpdateProduct(product);
            var response = new ApiResponse<ProductDto>(productDto);
            return Ok(response);
        }

        /// <summary>
        /// Elimina un producto. No permitido si tiene ventas asociadas.
        /// </summary>
        /// <param name="id">ID del producto a eliminar.</param>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Producto no encontrado.");

            await _productService.DeleteProduct(id);
            var response = new ApiResponse<string>($"Producto con ID {id} eliminado correctamente.");
            return Ok(response);
        }

        /// <summary>
        /// Registra el ingreso de mercadería al stock de un producto (CU-01).
        /// RN-06: la cantidad debe ser mayor a cero.
        /// </summary>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [HttpPost("ingreso-inventario")]
        public async Task<IActionResult> AddInventoryIngress(InventoryIngressDto dto)
        {
            var validationResult = await _inventoryValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _productService.AddInventoryIngressAsync(dto);
            var response = new ApiResponse<string>($"Ingreso registrado. Se agregaron {dto.Quantity} unidades al stock.");
            return Ok(response);
        }

        [HttpGet("dapper")]
        public async Task<IActionResult> GetAvailableDapper([FromQuery] int limit = 10)
        {
            var products = await _productService.GetAvailableProductsDapperAsync(limit);
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            var response = new ApiResponse<IEnumerable<ProductDto>>(productsDto);
            return Ok(response);
        }
    }
}
