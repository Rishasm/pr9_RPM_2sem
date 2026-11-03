using AutoMapper;
using HotelAPI.DTOs;
using HotelAPI.Models;
using HotelAPI.Repositories;

namespace HotelAPI.Services;

public class ProductReservationService : IProductReservationService
{
    private readonly IProductReservationRepository _repository;
    private readonly IProductRepository _products;
    private readonly IUserRepository _users;
    private readonly IMapper _mapper;

    public ProductReservationService(IProductReservationRepository repository,
        IProductRepository products, IUserRepository users, IMapper mapper)
    {
        _repository = repository;
        _products = products;
        _users = users;
        _mapper = mapper;
    }

    public Task<ProductReservation?> GetResourceAsync(int id, CancellationToken ct) => _repository.GetByIdAsync(id, ct);
    public async Task<(List<ProductReservationDTO> Items, int TotalCount)> GetVisibleAsync(int page, int pageSize, int? userId, CancellationToken ct)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100 || (long)(page - 1) * pageSize > int.MaxValue)
            throw new InvalidOperationException("Некорректные параметры пагинации.");
        return (_mapper.Map<List<ProductReservationDTO>>(await _repository.GetVisibleAsync(page, pageSize, userId, ct)),
            await _repository.CountVisibleAsync(userId, ct));
    }
    public async Task<(List<ProductReservationDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            throw new InvalidOperationException("Некорректные параметры пагинации.");
        var items = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<ProductReservationDTO>>(items), total);
    }

    public async Task<ProductReservationDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<ProductReservationDTO>(entity);
    }

    public async Task<ProductReservationDTO?> ReserveAsync(CreateProductReservationDTO dto, CancellationToken ct)
    {
        var product = await _products.GetByIdAsync(dto.ProductId, ct);
        if (product is null) return null;
        var user = await _users.GetByIdAsync(dto.UserId, ct);
        if (user is null) return null;
        if (dto.Quantity <= 0)
            throw new InvalidOperationException("Количество должно быть больше нуля.");
        if (product.AvailableQuantity < dto.Quantity)
            throw new InvalidOperationException("Недостаточно доступного товара для резервирования.");

        var reservation = _mapper.Map<ProductReservation>(dto);
        reservation.Product = product;
        reservation.User = user;
        reservation.CreatedAt = DateTime.UtcNow;
        product.AvailableQuantity -= dto.Quantity;
        await _repository.AddAsync(reservation, ct);
        await _repository.SaveChangesAsync(ct);
        return _mapper.Map<ProductReservationDTO>(reservation);
    }

    public async Task<bool> CancelAsync(int id, CancellationToken ct)
    {
        var reservation = await _repository.GetByIdAsync(id, ct);
        if (reservation is null || reservation.CancelledAt is not null) return false;
        if (reservation.Product.AvailableQuantity > int.MaxValue - reservation.Quantity)
            throw new InvalidOperationException("Возврат превышает допустимый остаток товара.");

        reservation.Product.AvailableQuantity += reservation.Quantity;
        reservation.CancelledAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync(ct);
        return true;
    }
}
