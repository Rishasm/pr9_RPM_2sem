namespace HotelAPI.DTOs;

public class UserSummaryDTO
{
    public int IdUser { get; set; }
    public string Login { get; set; } = null!;
    public string? UserName { get; set; }
}
