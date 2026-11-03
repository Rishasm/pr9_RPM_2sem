namespace HotelAPI.DTOs
{
    public class CartDTO
    {
        public int IdCart { get; set; }
        public int UserId { get; set; }
        public UserSummaryDTO User { get; set; } = null!;
    }
}
