using HotelAPI.Models;

namespace HotelAPI.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<User?> GetByIdAsync(int id, CancellationToken ct);
    Task<User?> FindByLoginAsync(string value, CancellationToken ct);
    Task<bool> IsReferencedAsync(int id, CancellationToken ct);
    Task AddAsync(User entity, CancellationToken ct);
    void Update(User entity);
    void Remove(User entity);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
