using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly HotelContext _context;

    public OrderItemRepository(HotelContext context) => _context = context;

    private IQueryable<OrderItem> Query() => _context.OrderItems.Include(x => x.Order).Include(x => x.Product);

    public Task<List<OrderItem>> GetAllAsync(int page, int pageSize, CancellationToken ct) =>
        Query().OrderBy(x => x.IdOrderItem).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public Task<int> CountAsync(CancellationToken ct) => _context.OrderItems.CountAsync(ct);

    public Task<OrderItem?> GetByIdAsync(int id, CancellationToken ct) =>
        Query().FirstOrDefaultAsync(x => x.IdOrderItem == id, ct);

    public Task<bool> IsReferencedAsync(int id, CancellationToken ct) =>
        Task.FromResult(false);

    public async Task AddAsync(OrderItem entity, CancellationToken ct) =>
        await _context.OrderItems.AddAsync(entity, ct);

    // Entities returned by this repository are tracked by the request's scoped context.
    public void Update(OrderItem entity) => _context.Entry(entity).State = EntityState.Modified;

    public void Remove(OrderItem entity) => _context.OrderItems.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
