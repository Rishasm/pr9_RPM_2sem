namespace HotelAPI.DTOs
{
    public class UpdateOrderDTO
    {
        public int UserId { get; set; }
        public DateOnly OrderDate { get; set; }
    }
}
