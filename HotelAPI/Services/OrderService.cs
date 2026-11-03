using AutoMapper;
using HotelAPI.DTOs;
using HotelAPI.Models;
using HotelAPI.Repositories;

namespace HotelAPI.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public OrderService(IOrderRepository repository, IMapper mapper, IUserRepository userRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<(List<OrderDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            throw new InvalidOperationException("Некорректные параметры пагинации.");
        var entities = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<OrderDTO>>(entities), total);
    }

    public async Task<OrderDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<OrderDTO>(entity);
    }

    public async Task<OrderDTO?> CreateAsync(CreateOrderDTO dto, CancellationToken ct)
    {
        var entity = _mapper.Map<Order>(dto);
        if (!await ValidateAsync(entity, ct)) return null;
        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
        return await GetByIdAsync(entity.IdOrder, ct);
    }

    public async Task<bool> UpdateAsync(int id, UpdateOrderDTO dto, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        _mapper.Map(dto, entity);
        if (!await ValidateAsync(entity, ct)) return false;
        _repository.Update(entity);
        await _repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> PatchAsync(int id, PatchOrderDTO dto, CancellationToken ct)
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

    private async Task<bool> ValidateAsync(Order entity, CancellationToken ct)
    {

        if (await _userRepository.GetByIdAsync(entity.UserId, ct) is null) return false;
        return true;
    }
}
