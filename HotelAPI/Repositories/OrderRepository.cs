using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly HotelContext _context;

    public OrderRepository(HotelContext context) => _context = context;

    private IQueryable<Order> Query() => _context.Orders.Include(x => x.User).Include(x => x.OrderItems).ThenInclude(x => x.Product);

    public Task<List<Order>> GetAllAsync(int page, int pageSize, CancellationToken ct) =>
        Query().OrderBy(x => x.IdOrder).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public Task<int> CountAsync(CancellationToken ct) => _context.Orders.CountAsync(ct);

    public Task<Order?> GetByIdAsync(int id, CancellationToken ct) =>
        Query().FirstOrDefaultAsync(x => x.IdOrder == id, ct);

    public async Task<bool> IsReferencedAsync(int id, CancellationToken ct) =>
        await _context.OrderItems.AnyAsync(x => x.OrderId == id, ct);

    public async Task AddAsync(Order entity, CancellationToken ct) =>
        await _context.Orders.AddAsync(entity, ct);

    // Entities returned by this repository are tracked by the request's scoped context.
    public void Update(Order entity) => _context.Entry(entity).State = EntityState.Modified;

    public void Remove(Order entity) => _context.Orders.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
