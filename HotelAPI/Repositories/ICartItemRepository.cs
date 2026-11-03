using HotelAPI.Models;

namespace HotelAPI.Repositories;

public interface ICartItemRepository
{
    Task<List<CartItem>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<CartItem?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> IsReferencedAsync(int id, CancellationToken ct);
    Task AddAsync(CartItem entity, CancellationToken ct);
    void Update(CartItem entity);
    void Remove(CartItem entity);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
