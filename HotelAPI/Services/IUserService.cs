using HotelAPI.DTOs;

namespace HotelAPI.Services;

public interface IUserService
{
    Task<HotelAPI.Models.User> RegisterAsync(string login, string password, CancellationToken ct);
    Task<HotelAPI.Models.User?> ValidateCredentialsAsync(string login, string password, CancellationToken ct);
    Task<(List<UserDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<UserDTO?> GetByIdAsync(int id, CancellationToken ct);
    Task<UserDTO?> CreateAsync(CreateUserDTO dto, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdateUserDTO dto, CancellationToken ct);
    Task<bool> PatchAsync(int id, PatchUserDTO dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
