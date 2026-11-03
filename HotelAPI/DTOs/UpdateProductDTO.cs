namespace HotelAPI.DTOs
{
    public class UpdateProductDTO
    {
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string? ImagePath { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
