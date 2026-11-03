namespace HotelAPI.DTOs
{
    public class CategoryDTO
    {
        public int IdCategory { get; set; }
        public string CategoryName { get; set; } = null!;
        public List<ProductSummaryDTO> Products { get; set; } = new();
    }
}
