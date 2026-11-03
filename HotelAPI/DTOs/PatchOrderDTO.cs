namespace HotelAPI.DTOs
{
    public class PatchOrderDTO
    {
        public int? UserId { get; set; }
        public DateOnly? OrderDate { get; set; }
    }
}