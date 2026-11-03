using HotelAPI.Models;

namespace HotelAPI.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<Order?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> IsReferencedAsync(int id, CancellationToken ct);
    Task AddAsync(Order entity, CancellationToken ct);
    void Update(Order entity);
    void Remove(Order entity);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
