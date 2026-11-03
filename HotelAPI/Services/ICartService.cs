using HotelAPI.DTOs;

namespace HotelAPI.Services;

public interface ICartService
{
    Task<(List<CartDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<CartDTO?> GetByIdAsync(int id, CancellationToken ct);
    Task<CartDTO?> CreateAsync(CreateCartDTO dto, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdateCartDTO dto, CancellationToken ct);
    Task<bool> PatchAsync(int id, PatchCartDTO dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
