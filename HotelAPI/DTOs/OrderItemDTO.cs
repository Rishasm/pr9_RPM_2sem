namespace HotelAPI.DTOs
{
    public class OrderItemDTO
    {
        public int IdOrderItem { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public OrderSummaryDTO Order { get; set; } = null!;
        public ProductSummaryDTO Product { get; set; } = null!;
    }
}
