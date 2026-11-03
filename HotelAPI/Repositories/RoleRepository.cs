using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly HotelContext _context;

    public RoleRepository(HotelContext context) => _context = context;

    private IQueryable<Role> Query() => _context.Roles.Include(x => x.Users);

    public Task<List<Role>> GetAllAsync(int page, int pageSize, CancellationToken ct) =>
        Query().OrderBy(x => x.IdRole).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public Task<int> CountAsync(CancellationToken ct) => _context.Roles.CountAsync(ct);

    public Task<Role?> GetByIdAsync(int id, CancellationToken ct) =>
        Query().FirstOrDefaultAsync(x => x.IdRole == id, ct);

    public Task<Role?> FindByRoleNameAsync(string value, CancellationToken ct) =>
        _context.Roles.FirstOrDefaultAsync(x => x.RoleName == value, ct);

    public async Task<bool> IsReferencedAsync(int id, CancellationToken ct) =>
        await _context.Users.AnyAsync(x => x.RoleId == id, ct);

    public async Task AddAsync(Role entity, CancellationToken ct) =>
        await _context.Roles.AddAsync(entity, ct);

    // Entities returned by this repository are tracked by the request's scoped context.
    public void Update(Role entity) => _context.Entry(entity).State = EntityState.Modified;

    public void Remove(Role entity) => _context.Roles.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
