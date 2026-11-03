namespace HotelAPI.DTOs
{
    public class PatchOrderItemDTO
    {
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
    }
}