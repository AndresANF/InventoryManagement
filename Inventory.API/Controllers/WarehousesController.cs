using Inventory.API.Models;
using Inventory.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly InventoryService _inventoryService;

        public WarehousesController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetWarehouses()
        {
            var warehouses = await _inventoryService.GetWarehousesAsync();
            return Ok(warehouses);
        }

        [HttpPost]
        public async Task<ActionResult<Warehouse>> PostWarehouse(Warehouse warehouse)
        {
            try
            {
                await _inventoryService.CreateWarehouseAsync(warehouse);
                return CreatedAtAction(nameof(GetWarehouses), new { id = warehouse.Id }, warehouse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarehouse(int id, Warehouse warehouse)
        {
            if (id != warehouse.Id)
            {
                return BadRequest("El ID del almacén no coincide.");
            }

            try
            {
                await _inventoryService.UpdateWarehouseAsync(warehouse);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            try
            {
                await _inventoryService.DeleteWarehouseAsync(id);
                return Ok(new { message = "Almacén eliminado." });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}