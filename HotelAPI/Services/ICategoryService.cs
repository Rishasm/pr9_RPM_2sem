using HotelAPI.DTOs;

namespace HotelAPI.Services;

public interface ICategoryService
{
    Task<(List<CategoryDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<CategoryDTO?> GetByIdAsync(int id, CancellationToken ct);
    Task<CategoryDTO?> CreateAsync(CreateCategoryDTO dto, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdateCategoryDTO dto, CancellationToken ct);
    Task<bool> PatchAsync(int id, PatchCategoryDTO dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
