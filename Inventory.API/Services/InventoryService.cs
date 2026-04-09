using Inventory.API.Data;
using Inventory.API.Models;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.IO;

namespace Inventory.API.Services
{
    public class InventoryService
    {
        private readonly InventoryDbContext _context;

        public InventoryService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                // No borrar si tiene productos
                var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
                if (hasProducts) throw new Exception("No se puede eliminar: Esta categoría tiene productos asociados.");

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // No borrar si tiene movimientos (Kardex)
                var hasMovements = await _context.InventoryMovements.AnyAsync(m => m.ProductId == id);
                if (hasMovements) throw new Exception("No se puede eliminar: El producto ya tiene historial de movimientos.");

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteWarehouseAsync(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse != null)
            {
                // No borrar si el almacén fue usado en algún movimiento
                var hasMovements = await _context.InventoryMovements.AnyAsync(m => m.WarehouseId == id);
                if (hasMovements) throw new Exception("No se puede eliminar: El almacén tiene registros de inventario vinculados.");

                _context.Warehouses.Remove(warehouse);
                await _context.SaveChangesAsync();
            }
        }

        #region Stock y Movimientos

        public async Task<List<object>> GetStockSummaryAsync()
        {
            var summary = await _context.InventoryMovements
                .Include(m => m.Product)
                .Include(m => m.Warehouse)
                .Where(m => m.Product != null && m.Warehouse != null)
                .GroupBy(m => new {
                    m.ProductId,
                    m.Product.Code,
                    m.Product.Name,
                    WhName = m.Warehouse.Name,
                    m.Product.MinimumStock
                })
                .Select(g => new
                {
                    productCode = g.Key.Code,
                    productName = g.Key.Name,
                    warehouseName = g.Key.WhName,
                    currentStock = g.Sum(m => m.MovementType == "Entrada" ? m.Quantity : -m.Quantity),
                    minimumStock = g.Key.MinimumStock
                })
                .ToListAsync();

            return summary.Cast<object>().ToList();
        }

        public async Task RegisterMovementAsync(int productId, int warehouseId, int quantity, string movementType, int? destinationWarehouseId = null)
        {
            // Usamos una transacción para asegurar que ambos registros se guarden correctamente
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validar Stock si es una Salida (Seguridad nivel Servidor)
                if (movementType == "Salida")
                {
                    var currentStock = await _context.InventoryMovements
                        .Where(m => m.ProductId == productId && m.WarehouseId == warehouseId)
                        .SumAsync(m => m.MovementType == "Entrada" ? m.Quantity : -m.Quantity);

                    if (currentStock < quantity)
                    {
                        throw new Exception("Stock insuficiente en el almacén de origen para realizar la operación.");
                    }
                }

                // 2. Registrar el movimiento en el origen
                var originMovement = new InventoryMovement
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    Quantity = quantity,
                    MovementType = movementType, // "Entrada" o "Salida"
                    Date = DateTime.Now
                };
                _context.InventoryMovements.Add(originMovement);

                // 3. Si es Salida y tiene destino, crear automáticamente la Entrada en el destino
                if (movementType == "Salida" && destinationWarehouseId.HasValue && destinationWarehouseId.Value != 0)
                {
                    var targetMovement = new InventoryMovement
                    {
                        ProductId = productId,
                        WarehouseId = destinationWarehouseId.Value,
                        Quantity = quantity,
                        MovementType = "Entrada", // Siempre entra en el destino
                        Date = DateTime.Now
                    };
                    _context.InventoryMovements.Add(targetMovement);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region Gestión de Entidades (Categorías, Productos, Almacenes)

        // Categorías
        public async Task<List<Category>> GetCategoriesAsync() => await _context.Categories.ToListAsync();
        public async Task CreateCategoryAsync(Category category) { _context.Categories.Add(category); await _context.SaveChangesAsync(); }
        public async Task UpdateCategoryAsync(Category category) { _context.Entry(category).State = EntityState.Modified; await _context.SaveChangesAsync(); }

        // Productos
        public async Task<List<Product>> GetProductsAsync() => await _context.Products.ToListAsync();
        public async Task CreateProductAsync(Product product) { _context.Products.Add(product); await _context.SaveChangesAsync(); }
        public async Task UpdateProductAsync(Product product) { _context.Entry(product).State = EntityState.Modified; await _context.SaveChangesAsync(); }

        // Almacenes
        public async Task<List<Warehouse>> GetWarehousesAsync() => await _context.Warehouses.ToListAsync();
        public async Task CreateWarehouseAsync(Warehouse warehouse) { _context.Warehouses.Add(warehouse); await _context.SaveChangesAsync(); }
        public async Task UpdateWarehouseAsync(Warehouse warehouse) { _context.Entry(warehouse).State = EntityState.Modified; await _context.SaveChangesAsync(); }

        #endregion

        #region Reporte Excel Formateado
        public async Task<byte[]> GenerateExcelReportAsync(string productCode)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Reporte de Movimientos");
            string[] headers = { "Código Producto", "Descripción", "Fecha", "Nro Movimiento", "Tipo", "Almacén", "Cantidad" };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#D3D3D3");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var movs = await _context.InventoryMovements
                .Include(m => m.Product)
                .Include(m => m.Warehouse)
                .Where(m => m.Product.Code == productCode)
                .OrderBy(m => m.Date)
                .ToListAsync();

            for (int i = 0; i < movs.Count; i++)
            {
                int row = i + 2;
                var m = movs[i];
                ws.Cell(row, 1).Value = m.Product.Code;
                ws.Cell(row, 2).Value = m.Product.Name;
                ws.Cell(row, 3).Value = m.Date.ToString("dd/MM/yyyy HH:mm");
                var cellNro = ws.Cell(row, 4);
                cellNro.Value = i + 1;
                cellNro.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 5).Value = m.MovementType;
                ws.Cell(row, 6).Value = m.Warehouse.Name;
                var cellQty = ws.Cell(row, 7);
                cellQty.Value = m.Quantity;
                cellQty.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Range(row, 1, row, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            ws.Columns().AdjustToContents();
            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }
        #endregion
    }
}