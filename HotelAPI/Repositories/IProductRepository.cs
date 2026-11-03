using HotelAPI.Models;

namespace HotelAPI.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> IsReferencedAsync(int id, CancellationToken ct);
    Task AddAsync(Product entity, CancellationToken ct);
    void Update(Product entity);
    void Remove(Product entity);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
