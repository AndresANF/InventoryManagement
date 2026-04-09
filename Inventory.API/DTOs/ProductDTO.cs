namespace Inventory.API.DTOs
{
    public class ProductDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }
}
