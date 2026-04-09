using Inventory.API.Models;
using Inventory.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly InventoryService _inventoryService;

        public CategoriesController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _inventoryService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            try
            {
                await _inventoryService.CreateCategoryAsync(category);
                return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id) return BadRequest();
            await _inventoryService.UpdateCategoryAsync(category);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _inventoryService.DeleteCategoryAsync(id);
                return Ok(new { message = "Categoría eliminada." });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}