namespace Inventory.API.Models
{
    public class Inventory
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Stock { get; set; }

        public virtual Product Product { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}