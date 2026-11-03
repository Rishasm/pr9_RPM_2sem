using HotelAPI.DTOs;

namespace HotelAPI.Services;

public interface IOrderItemService
{
    Task<(List<OrderItemDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<OrderItemDTO?> GetByIdAsync(int id, CancellationToken ct);
    Task<OrderItemDTO?> CreateAsync(CreateOrderItemDTO dto, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdateOrderItemDTO dto, CancellationToken ct);
    Task<bool> PatchAsync(int id, PatchOrderItemDTO dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
