using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly HotelContext _context;

    public CategoryRepository(HotelContext context) => _context = context;

    private IQueryable<Category> Query() => _context.Categories.Include(x => x.Products);

    public Task<List<Category>> GetAllAsync(int page, int pageSize, CancellationToken ct) =>
        Query().OrderBy(x => x.IdCategory).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public Task<int> CountAsync(CancellationToken ct) => _context.Categories.CountAsync(ct);

    public Task<Category?> GetByIdAsync(int id, CancellationToken ct) =>
        Query().FirstOrDefaultAsync(x => x.IdCategory == id, ct);

    public Task<Category?> FindByCategoryNameAsync(string value, CancellationToken ct) =>
        _context.Categories.FirstOrDefaultAsync(x => x.CategoryName == value, ct);

    public async Task<bool> IsReferencedAsync(int id, CancellationToken ct) =>
        await _context.Products.AnyAsync(x => x.CategoryId == id, ct);

    public async Task AddAsync(Category entity, CancellationToken ct) =>
        await _context.Categories.AddAsync(entity, ct);

    // Entities returned by this repository are tracked by the request's scoped context.
    public void Update(Category entity) => _context.Entry(entity).State = EntityState.Modified;

    public void Remove(Category entity) => _context.Categories.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
