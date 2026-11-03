namespace HotelAPI.DTOs
{
    public class CartItemDTO
    {
        public int IdCartItem { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public CartSummaryDTO? Cart { get; set; }
        public ProductSummaryDTO? Product { get; set; }
    }
}
