using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Repositories;

public class CartItemRepository : ICartItemRepository
{
    private readonly HotelContext _context;

    public CartItemRepository(HotelContext context) => _context = context;

    private IQueryable<CartItem> Query() => _context.CartItems;

    public async Task<List<CartItem>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        var items = await Query().OrderBy(x => x.IdCartItem)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        await LoadRelatedAsync(items, ct);
        return items;
    }

    public Task<int> CountAsync(CancellationToken ct) => _context.CartItems.CountAsync(ct);

    public async Task<CartItem?> GetByIdAsync(int id, CancellationToken ct)
    {
        var item = await Query().FirstOrDefaultAsync(x => x.IdCartItem == id, ct);
        if (item is not null) await LoadRelatedAsync(new[] { item }, ct);
        return item;
    }

    private async Task LoadRelatedAsync(IReadOnlyCollection<CartItem> items, CancellationToken ct)
    {
        if (items.Count == 0) return;
        var cartIds = items.Select(x => x.CartId).Distinct().ToArray();
        var productIds = items.Select(x => x.ProductId).Distinct().ToArray();
        var carts = await _context.Carts.Where(x => cartIds.Contains(x.IdCart)).ToDictionaryAsync(x => x.IdCart, ct);
        var products = await _context.Products.Where(x => productIds.Contains(x.IdProduct)).ToDictionaryAsync(x => x.IdProduct, ct);
        foreach (var item in items)
        {
            item.Cart = carts.GetValueOrDefault(item.CartId);
            item.Product = products.GetValueOrDefault(item.ProductId);
        }
    }

    public Task<bool> IsReferencedAsync(int id, CancellationToken ct) =>
        Task.FromResult(false);

    public async Task AddAsync(CartItem entity, CancellationToken ct) =>
        await _context.CartItems.AddAsync(entity, ct);

    // Entities returned by this repository are tracked by the request's scoped context.
    public void Update(CartItem entity) => _context.Entry(entity).State = EntityState.Modified;

    public void Remove(CartItem entity) => _context.CartItems.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
