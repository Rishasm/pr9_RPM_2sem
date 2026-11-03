using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelAPI.Models;
using Microsoft.IdentityModel.Tokens;
namespace HotelAPI.Services;
public interface ITokenService { string CreateAccessToken(User user); }
public class TokenService(IConfiguration configuration) : ITokenService
{
    public string CreateAccessToken(User user)
    {
        var jwt = configuration.GetSection("Jwt");
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.RoleName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var token = new JwtSecurityToken(jwt["Issuer"], jwt["Audience"], claims,
            expires: DateTime.UtcNow.AddMinutes(jwt.GetValue<int>("ExpiresMinutes")),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)), SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

