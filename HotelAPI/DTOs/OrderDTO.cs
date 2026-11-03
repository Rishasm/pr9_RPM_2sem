namespace HotelAPI.DTOs
{
    public class OrderDTO
    {
        public int IdOrder { get; set; }
        public int UserId { get; set; }
        public DateOnly OrderDate { get; set; }
        public UserSummaryDTO User { get; set; } = null!;
        public List<OrderItemDTO> OrderItems { get; set; } = new();
    }
}
