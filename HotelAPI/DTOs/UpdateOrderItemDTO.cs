namespace HotelAPI.DTOs
{
    public class UpdateOrderItemDTO
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}