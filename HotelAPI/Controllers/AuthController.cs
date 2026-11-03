using System.Security.Claims;
using HotelAPI.DTOs;
using HotelAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace HotelAPI.Controllers;
[ApiController, Route("api/auth")]
public class AuthController(IUserService users, ITokenService tokens, IConfiguration config) : ControllerBase
{
    private LoginResponseDTO Token(HotelAPI.Models.User user) =>
        new(tokens.CreateAccessToken(user), config.GetValue<int>("Jwt:ExpiresMinutes"));
    [AllowAnonymous, HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto, CancellationToken ct)
    {
        try { return Ok(Token(await users.RegisterAsync(dto.Login, dto.Password, ct))); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }
    [AllowAnonymous, HttpPost("login"), EnableRateLimiting("LoginPolicy")]
    public async Task<IActionResult> Login(LoginDTO dto, CancellationToken ct)
    {
        var user = await users.ValidateCredentialsAsync(dto.Login, dto.Password, ct);
        return user is null ? Unauthorized(new { error = "Неверный логин или пароль, либо учётная запись отключена." }) : Ok(Token(user));
    }
    [Authorize, HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct) =>
        Ok(await users.GetByIdAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!), ct));
}

