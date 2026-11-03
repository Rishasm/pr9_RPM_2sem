using HotelAPI.DTOs;

namespace HotelAPI.Services;

public interface IRoleService
{
    Task<(List<RoleDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<RoleDTO?> GetByIdAsync(int id, CancellationToken ct);
    Task<RoleDTO?> CreateAsync(CreateRoleDTO dto, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdateRoleDTO dto, CancellationToken ct);
    Task<bool> PatchAsync(int id, PatchRoleDTO dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
