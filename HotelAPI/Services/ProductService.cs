using AutoMapper;
using HotelAPI.DTOs;
using HotelAPI.Models;
using HotelAPI.Repositories;

namespace HotelAPI.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository repository, IMapper mapper, ICategoryRepository categoryRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _categoryRepository = categoryRepository;
    }

    public async Task<(List<ProductDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            throw new InvalidOperationException("Некорректные параметры пагинации.");
        var entities = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<ProductDTO>>(entities), total);
    }

    public async Task<ProductDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<ProductDTO>(entity);
    }

    public async Task<ProductDTO?> CreateAsync(CreateProductDTO dto, CancellationToken ct)
    {
        var entity = _mapper.Map<Product>(dto);
        if (!await ValidateAsync(entity, ct)) return null;
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
        return await GetByIdAsync(entity.IdProduct, ct);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductDTO dto, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        _mapper.Map(dto, entity);
        if (!await ValidateAsync(entity, ct)) return false;
        _repository.Update(entity);
        await _repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> PatchAsync(int id, PatchProductDTO dto, CancellationToken ct)
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

    private async Task<bool> ValidateAsync(Product entity, CancellationToken ct)
    {
        if (entity.Price <= 0) throw new InvalidOperationException("Цена должна быть больше нуля.");
        if (entity.AvailableQuantity < 0) throw new InvalidOperationException("Остаток не может быть отрицательным.");
        if (string.IsNullOrWhiteSpace(entity.ProductName) || entity.ProductName.Length > 150)
            throw new InvalidOperationException("Поле ProductName обязательно и должно содержать не более 150 символов.");
        if (entity.Description?.Length > 500) throw new InvalidOperationException("Поле Description: не более 500 символов.");
        if (entity.ImagePath?.Length > 300) throw new InvalidOperationException("Поле ImagePath: не более 300 символов.");
        if (await _categoryRepository.GetByIdAsync(entity.CategoryId, ct) is null) return false;
        return true;
    }
}
