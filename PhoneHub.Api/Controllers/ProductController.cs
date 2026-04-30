using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PhoneHub.Api.Responses;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.QueryFilters;
using PhoneHub.Services.Interfaces;
using PhoneHub.Services.Validators;

namespace PhoneHub.Api.Controllers
{
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

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductQueryFilter? filters)
        {
            var products = await _productService.GetAllProductsAsync(filters);
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            var response = new ApiResponse<IEnumerable<ProductDto>>(productsDto);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Producto no encontrado.");

            await _productService.DeleteProduct(id);
            return NoContent();
        }

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
    }
}
