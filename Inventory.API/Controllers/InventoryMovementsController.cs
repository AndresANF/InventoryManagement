using Inventory.API.Models;
using Inventory.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryMovementsController : ControllerBase
    {
        private readonly InventoryService _inventoryService;

        public InventoryMovementsController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // DTO para mapear el JSON exacto que envía Angular
        public class MovementRequestDTO
        {
            public int ProductId { get; set; }
            public int WarehouseId { get; set; } // Origen
            public int? DestinationWarehouseId { get; set; } // Destino (Obligatorio en Salida/Movimiento)
            public int Quantity { get; set; }
            public string Type { get; set; } // "Entrada" o "Salida"
        }

        [HttpGet("stock-summary")]
        public async Task<IActionResult> GetStockSummary()
        {
            var summary = await _inventoryService.GetStockSummaryAsync();
            return Ok(summary);
        }

        [HttpPost("movement")]
        public async Task<IActionResult> RegisterMovement([FromBody] MovementRequestDTO request)
        {
            try
            {
                // Validaciones básicas de integridad
                if (request == null || request.ProductId <= 0 || request.WarehouseId <= 0 || request.Quantity <= 0)
                {
                    return BadRequest(new { message = "Datos de movimiento inválidos o incompletos." });
                }

                // Validación específica para Salida (Movimiento entre almacenes)
                if (request.Type == "Salida")
                {
                    if (request.DestinationWarehouseId == null || request.DestinationWarehouseId == 0)
                    {
                        return BadRequest(new { message = "Debe seleccionar un almacén de destino para realizar la salida." });
                    }

                    if (request.WarehouseId == request.DestinationWarehouseId)
                    {
                        return BadRequest(new { message = "El almacén de origen y destino no pueden ser el mismo." });
                    }
                }

                // Procesar en el servicio
                await _inventoryService.RegisterMovementAsync(
                    request.ProductId,
                    request.WarehouseId,
                    request.Quantity,
                    request.Type,
                    request.DestinationWarehouseId);

                return Ok(new { message = $"Movimiento de {request.Type} registrado con éxito." });
            }
            catch (Exception ex)
            {
                // Capturamos el error (ej: Stock insuficiente) y lo enviamos a Angular
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("report/excel")]
        public async Task<IActionResult> DownloadExcel(string productCode)
        {
            try
            {
                var fileData = await _inventoryService.GenerateExcelReportAsync(productCode);
                return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Reporte_{productCode}.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}