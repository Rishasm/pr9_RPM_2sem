using AutoMapper;
using HotelAPI.DTOs;
using HotelAPI.Models;
using HotelAPI.Repositories;

namespace HotelAPI.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public CartService(ICartRepository repository, IMapper mapper, IUserRepository userRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<(List<CartDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            throw new InvalidOperationException("Некорректные параметры пагинации.");
        var entities = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<CartDTO>>(entities), total);
    }

    public async Task<CartDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<CartDTO>(entity);
    }

    public async Task<CartDTO?> CreateAsync(CreateCartDTO dto, CancellationToken ct)
    {
        if (dto.IdCart is null) throw new InvalidOperationException("Укажите IdCart для создания записи.");
        if (await _repository.GetByIdAsync(dto.IdCart.Value, ct) is not null)
            throw new InvalidOperationException("Ключ IdCart уже занят.");
        var entity = _mapper.Map<Cart>(dto);
        if (!await ValidateAsync(entity, ct)) return null;
        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
        return await GetByIdAsync(entity.IdCart, ct);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCartDTO dto, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        _mapper.Map(dto, entity);
        if (!await ValidateAsync(entity, ct)) return false;
        _repository.Update(entity);
        await _repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> PatchAsync(int id, PatchCartDTO dto, CancellationToken ct)
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

    private async Task<bool> ValidateAsync(Cart entity, CancellationToken ct)
    {
        var existing = await _repository.FindByUserIdAsync(entity.UserId, ct);
        if (existing is not null && existing.IdCart != entity.IdCart)
            throw new InvalidOperationException("Значение UserId уже используется.");
        if (await _userRepository.GetByIdAsync(entity.UserId, ct) is null) return false;
        return true;
    }
}
