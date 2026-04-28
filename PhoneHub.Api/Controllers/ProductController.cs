using AutoMapper;
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
                var product = _mapper.Map<Product>(productDto);
                await _productService.InsertProduct(product);
                var createdDto = _mapper.Map<ProductDto>(product);
                var response = new ApiResponse<ProductDto>(createdDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el producto", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto productDto)
        {
            if (id != productDto.Id)
                return BadRequest("El ID del producto no coincide.");

            var validationResult = await _actualizarValidator.ValidateAsync(productDto);
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

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Producto no encontrado.");

            try
            {
                _mapper.Map(productDto, product);
                await _productService.UpdateProduct(product);
                var response = new ApiResponse<ProductDto>(productDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el producto", error = ex.Message });
            }
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
                var result = await _productService.AddInventoryIngressAsync(dto);
                if (!result)
                    return NotFound(new { message = "Producto no encontrado" });

                var response = new ApiResponse<string>($"Ingreso registrado. Se agregaron {dto.Quantity} unidades al stock.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al registrar el ingreso", error = ex.Message });
            }
        }
    }
}
