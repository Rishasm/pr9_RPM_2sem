using HotelAPI.DTOs;

namespace HotelAPI.Services;

public interface IOrderService
{
    Task<(List<OrderDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<OrderDTO?> GetByIdAsync(int id, CancellationToken ct);
    Task<OrderDTO?> CreateAsync(CreateOrderDTO dto, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdateOrderDTO dto, CancellationToken ct);
    Task<bool> PatchAsync(int id, PatchOrderDTO dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
