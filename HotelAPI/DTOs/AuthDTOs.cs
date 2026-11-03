using System.ComponentModel.DataAnnotations;
namespace HotelAPI.DTOs;
public class RegisterDTO
{
    [Required, StringLength(50, MinimumLength = 3)] public string Login { get; set; } = "";
    [Required, StringLength(128, MinimumLength = 8)] public string Password { get; set; } = "";
}
public class LoginDTO
{
    [Required, StringLength(50)] public string Login { get; set; } = "";
    [Required, StringLength(128)] public string Password { get; set; } = "";
}
public record LoginResponseDTO(string AccessToken, int ExpiresInMinutes);

