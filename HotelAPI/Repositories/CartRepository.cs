using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Repositories;

public class CartRepository : ICartRepository
{
    private readonly HotelContext _context;

    public CartRepository(HotelContext context) => _context = context;

    private IQueryable<Cart> Query() => _context.Carts.Include(x => x.User);

    public Task<List<Cart>> GetAllAsync(int page, int pageSize, CancellationToken ct) =>
        Query().OrderBy(x => x.IdCart).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public Task<int> CountAsync(CancellationToken ct) => _context.Carts.CountAsync(ct);

    public Task<Cart?> GetByIdAsync(int id, CancellationToken ct) =>
        Query().FirstOrDefaultAsync(x => x.IdCart == id, ct);

    public Task<Cart?> FindByUserIdAsync(int value, CancellationToken ct) =>
        _context.Carts.FirstOrDefaultAsync(x => x.UserId == value, ct);

    public async Task<bool> IsReferencedAsync(int id, CancellationToken ct) =>
        await _context.CartItems.AnyAsync(x => x.CartId == id, ct);

    public async Task AddAsync(Cart entity, CancellationToken ct) =>
        await _context.Carts.AddAsync(entity, ct);

    // Entities returned by this repository are tracked by the request's scoped context.
    public void Update(Cart entity) => _context.Entry(entity).State = EntityState.Modified;

    public void Remove(Cart entity) => _context.Carts.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
