using HotelAPI.Models;

namespace HotelAPI.Repositories;

public interface ICartRepository
{
    Task<List<Cart>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<Cart?> GetByIdAsync(int id, CancellationToken ct);
    Task<Cart?> FindByUserIdAsync(int value, CancellationToken ct);
    Task<bool> IsReferencedAsync(int id, CancellationToken ct);
    Task AddAsync(Cart entity, CancellationToken ct);
    void Update(Cart entity);
    void Remove(Cart entity);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
