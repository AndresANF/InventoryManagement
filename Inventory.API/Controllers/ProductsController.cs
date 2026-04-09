using Inventory.API.Models;
using Inventory.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly InventoryService _inventoryService;

        public ProductsController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _inventoryService.GetProductsAsync();
            return Ok(products);
        }

        // --- AGREGAR ESTO PARA SOLUCIONAR EL ERROR 405 ---
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            try
            {
                await _inventoryService.CreateProductAsync(product);
                // Retornamos 201 Created y el objeto con su nuevo ID
                return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el producto: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("El ID del producto no coincide.");
            }

            try
            {
                await _inventoryService.UpdateProductAsync(product);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _inventoryService.DeleteProductAsync(id);
                return Ok(new { message = "Producto eliminado." });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}