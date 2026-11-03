using HotelAPI.Models;

namespace HotelAPI.Repositories;

public interface IOrderItemRepository
{
    Task<List<OrderItem>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<OrderItem?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> IsReferencedAsync(int id, CancellationToken ct);
    Task AddAsync(OrderItem entity, CancellationToken ct);
    void Update(OrderItem entity);
    void Remove(OrderItem entity);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
