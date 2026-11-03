namespace HotelAPI.DTOs;

public class CreateProductReservationDTO
{
    public int ProductId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public int UserId { get; set; }
    public int Quantity { get; set; }
}
