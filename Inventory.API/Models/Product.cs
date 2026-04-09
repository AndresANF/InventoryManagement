namespace Inventory.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int MinimumStock { get; set; }
        public bool IsActive { get; set; } = true;

        public Category? Category { get; set; }
    }
}