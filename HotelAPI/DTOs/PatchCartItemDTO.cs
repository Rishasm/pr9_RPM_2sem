namespace HotelAPI.DTOs
{
    public class PatchCartItemDTO
    {
        public int? CartId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
    }
}