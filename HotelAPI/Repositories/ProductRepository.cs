using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly HotelContext _context;

    public ProductRepository(HotelContext context) => _context = context;

    private IQueryable<Product> Query() => _context.Products.Include(x => x.Category);

    public Task<List<Product>> GetAllAsync(int page, int pageSize, CancellationToken ct) =>
        Query().OrderBy(x => x.IdProduct).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public Task<int> CountAsync(CancellationToken ct) => _context.Products.CountAsync(ct);

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct) =>
        Query().FirstOrDefaultAsync(x => x.IdProduct == id, ct);

    public async Task<bool> IsReferencedAsync(int id, CancellationToken ct) =>
        await _context.OrderItems.AnyAsync(x => x.ProductId == id, ct) || await _context.CartItems.AnyAsync(x => x.ProductId == id, ct) || await _context.ProductReservations.AnyAsync(x => x.ProductId == id, ct);

    public async Task AddAsync(Product entity, CancellationToken ct) =>
        await _context.Products.AddAsync(entity, ct);

    // Entities returned by this repository are tracked by the request's scoped context.
    public void Update(Product entity) => _context.Entry(entity).State = EntityState.Modified;

    public void Remove(Product entity) => _context.Products.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
