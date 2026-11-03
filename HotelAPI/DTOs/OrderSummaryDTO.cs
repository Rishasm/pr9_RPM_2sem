namespace HotelAPI.DTOs;

public class OrderSummaryDTO
{
    public int IdOrder { get; set; }
    public int UserId { get; set; }
    public DateOnly OrderDate { get; set; }
}
