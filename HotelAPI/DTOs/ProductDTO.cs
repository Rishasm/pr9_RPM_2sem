namespace HotelAPI.DTOs
{
    public class ProductDTO
    {
        public int IdProduct { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public CategorySummaryDTO Category { get; set; } = null!;
    }
}
