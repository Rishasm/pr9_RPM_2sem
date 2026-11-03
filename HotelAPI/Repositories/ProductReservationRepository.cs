using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Repositories;

public class ProductReservationRepository : IProductReservationRepository
{
    private readonly HotelContext _context;

    public ProductReservationRepository(HotelContext context) => _context = context;

    private IQueryable<ProductReservation> Query() =>
        _context.ProductReservations.Include(x => x.Product).Include(x => x.User);

    public Task<List<ProductReservation>> GetAllAsync(int page, int pageSize, CancellationToken ct) =>
        Query().OrderBy(x => x.IdProductReservation).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public Task<List<ProductReservation>> GetVisibleAsync(int page, int pageSize, int? userId, CancellationToken ct) =>
        Query().Where(x => userId == null || x.UserId == userId).OrderBy(x => x.IdProductReservation)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    public Task<int> CountVisibleAsync(int? userId, CancellationToken ct) =>
        _context.ProductReservations.CountAsync(x => userId == null || x.UserId == userId, ct);
    public Task<int> CountAsync(CancellationToken ct) => _context.ProductReservations.CountAsync(ct);

    public Task<ProductReservation?> GetByIdAsync(int id, CancellationToken ct) =>
        Query().FirstOrDefaultAsync(x => x.IdProductReservation == id, ct);

    public async Task AddAsync(ProductReservation entity, CancellationToken ct) =>
        await _context.ProductReservations.AddAsync(entity, ct);

    public Task<int> SaveChangesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
}
