using AutoMapper;
using HotelAPI.DTOs;
using HotelAPI.Models;
using HotelAPI.Repositories;

namespace HotelAPI.Services;

public class OrderItemService : IOrderItemService
{
    private readonly IOrderItemRepository _repository;
    private readonly IMapper _mapper;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrderItemService(IOrderItemRepository repository, IMapper mapper, IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<(List<OrderItemDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            throw new InvalidOperationException("Некорректные параметры пагинации.");
        var entities = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<OrderItemDTO>>(entities), total);
    }

    public async Task<OrderItemDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<OrderItemDTO>(entity);
    }

    public async Task<OrderItemDTO?> CreateAsync(CreateOrderItemDTO dto, CancellationToken ct)
    {
        if (dto.IdOrderItem is null) throw new InvalidOperationException("Укажите IdOrderItem для создания записи.");
        if (await _repository.GetByIdAsync(dto.IdOrderItem.Value, ct) is not null)
            throw new InvalidOperationException("Ключ IdOrderItem уже занят.");
        var entity = _mapper.Map<OrderItem>(dto);
        if (!await ValidateAsync(entity, ct)) return null;
        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
        return await GetByIdAsync(entity.IdOrderItem, ct);
    }

    public async Task<bool> UpdateAsync(int id, UpdateOrderItemDTO dto, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        _mapper.Map(dto, entity);
        if (!await ValidateAsync(entity, ct)) return false;
        _repository.Update(entity);
        await _repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> PatchAsync(int id, PatchOrderItemDTO dto, CancellationToken ct)
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

    private async Task<bool> ValidateAsync(OrderItem entity, CancellationToken ct)
    {
        if (entity.Quantity <= 0) throw new InvalidOperationException("Количество должно быть больше нуля.");
        if (await _orderRepository.GetByIdAsync(entity.OrderId, ct) is null) return false;
        if (await _productRepository.GetByIdAsync(entity.ProductId, ct) is null) return false;
        return true;
    }
}
