using HotelAPI.DTOs;

namespace HotelAPI.Services;

public interface IProductService
{
    Task<(List<ProductDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<ProductDTO?> GetByIdAsync(int id, CancellationToken ct);
    Task<ProductDTO?> CreateAsync(CreateProductDTO dto, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdateProductDTO dto, CancellationToken ct);
    Task<bool> PatchAsync(int id, PatchProductDTO dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
