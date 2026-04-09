namespace Inventory.API.Models
{
    public class InventoryMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public string MovementType { get; set; }
        public DateTime Date { get; set; }

        public virtual Product Product { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}