using Microsoft.AspNetCore.Mvc;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Interfaces;

namespace PhoneHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productRepository.GetProductsAsync();
            return Ok(products);
        }

        // GET: api/product/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Producto no encontrado" });
            return Ok(product);
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> Add(Product product)
        {
            await _productRepository.AddProductAsync(product);
            return Created($"api/product/{product.Id}", product);
        }

        // PUT: api/product
        [HttpPut]
        public async Task<IActionResult> Update(Product product)
        {
            await _productRepository.UpdateProductAsync(product);
            return NoContent();
        }

        // DELETE: api/product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productRepository.DeleteProductAsync(id);
            return NoContent();
        }

        // POST: api/product/ingreso-inventario
        // ✅ Requerimiento: Registrar Nuevo Ingreso de Inventario
        // Body ejemplo: { "productId": 1, "quantity": 10 }
        [HttpPost("ingreso-inventario")]
        public async Task<IActionResult> AddInventoryIngress(InventoryIngressDto dto)
        {
            var result = await _productRepository.AddInventoryIngressAsync(dto);
            if (!result)
                return NotFound(new { message = "Producto no encontrado" });

            return Ok(new { message = $"Ingreso registrado. Se agregaron {dto.Quantity} unidades al stock." });
        }
    }
}
