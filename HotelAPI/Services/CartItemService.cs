using AutoMapper;
using HotelAPI.DTOs;
using HotelAPI.Models;
using HotelAPI.Repositories;

namespace HotelAPI.Services;

public class CartItemService : ICartItemService
{
    private readonly ICartItemRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartItemService(ICartItemRepository repository, IMapper mapper, ICartRepository cartRepository, IProductRepository productRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<(List<CartItemDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            throw new InvalidOperationException("Некорректные параметры пагинации.");
        var entities = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<CartItemDTO>>(entities), total);
    }

    public async Task<CartItemDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<CartItemDTO>(entity);
    }

    public async Task<CartItemDTO?> CreateAsync(CreateCartItemDTO dto, CancellationToken ct)
    {
        var entity = _mapper.Map<CartItem>(dto);
        if (!await ValidateAsync(entity, ct)) return null;
        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
        return await GetByIdAsync(entity.IdCartItem, ct);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCartItemDTO dto, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        _mapper.Map(dto, entity);
        if (!await ValidateAsync(entity, ct)) return false;
        _repository.Update(entity);
        await _repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> PatchAsync(int id, PatchCartItemDTO dto, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        _mapper.Map(dto, entity);
        if (!await ValidateAsync(entity, ct)) return false;
        _repository.Update(entity);
        await _repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        if (await _repository.IsReferencedAsync(id, ct))
            throw new InvalidOperationException("Удаление невозможно: существуют связанные записи.");
        _repository.Remove(entity);
        await _repository.SaveChangesAsync(ct);
        return true;
    }

    private async Task<bool> ValidateAsync(CartItem entity, CancellationToken ct)
    {
        if (entity.Quantity <= 0) throw new InvalidOperationException("Количество должно быть больше нуля.");
        if (await _cartRepository.GetByIdAsync(entity.CartId, ct) is null) return false;
        if (await _productRepository.GetByIdAsync(entity.ProductId, ct) is null) return false;
        return true;
    }
}
