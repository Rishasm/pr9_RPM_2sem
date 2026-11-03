namespace HotelAPI.DTOs
{
    public class CreateOrderDTO
    {
        public int UserId { get; set; }
        public DateOnly OrderDate { get; set; }
    }
}