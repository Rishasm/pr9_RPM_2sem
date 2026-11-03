using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly HotelContext _context;

    public UserRepository(HotelContext context) => _context = context;

    private IQueryable<User> Query() => _context.Users.Include(x => x.Role).Include(x => x.Cart).Include(x => x.Orders);

    public Task<List<User>> GetAllAsync(int page, int pageSize, CancellationToken ct) =>
        Query().OrderBy(x => x.IdUser).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public Task<int> CountAsync(CancellationToken ct) => _context.Users.CountAsync(ct);

    public Task<User?> GetByIdAsync(int id, CancellationToken ct) =>
        Query().FirstOrDefaultAsync(x => x.IdUser == id, ct);

    public Task<User?> FindByLoginAsync(string value, CancellationToken ct) =>
        _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Login == value, ct);

    public async Task<bool> IsReferencedAsync(int id, CancellationToken ct) =>
        await _context.Carts.AnyAsync(x => x.UserId == id, ct) || await _context.Orders.AnyAsync(x => x.UserId == id, ct) || await _context.ProductReservations.AnyAsync(x => x.UserId == id, ct);

    public async Task AddAsync(User entity, CancellationToken ct) =>
        await _context.Users.AddAsync(entity, ct);

    // Entities returned by this repository are tracked by the request's scoped context.
    public void Update(User entity) => _context.Entry(entity).State = EntityState.Modified;

    public void Remove(User entity) => _context.Users.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
