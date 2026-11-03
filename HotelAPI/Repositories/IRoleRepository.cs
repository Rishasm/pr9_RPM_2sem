using HotelAPI.Models;

namespace HotelAPI.Repositories;

public interface IRoleRepository
{
    Task<List<Role>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<Role?> GetByIdAsync(int id, CancellationToken ct);
    Task<Role?> FindByRoleNameAsync(string value, CancellationToken ct);
    Task<bool> IsReferencedAsync(int id, CancellationToken ct);
    Task AddAsync(Role entity, CancellationToken ct);
    void Update(Role entity);
    void Remove(Role entity);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
