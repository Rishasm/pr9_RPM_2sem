using AutoMapper;
using HotelAPI.DTOs;
using HotelAPI.Models;
using HotelAPI.Repositories;

namespace HotelAPI.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;
    private readonly IMapper _mapper;

    public RoleService(IRoleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<(List<RoleDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            throw new InvalidOperationException("Некорректные параметры пагинации.");
        var entities = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<RoleDTO>>(entities), total);
    }

    public async Task<RoleDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<RoleDTO>(entity);
    }

    public async Task<RoleDTO?> CreateAsync(CreateRoleDTO dto, CancellationToken ct)
    {
        var entity = _mapper.Map<Role>(dto);
        if (!await ValidateAsync(entity, ct)) return null;
        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
        return await GetByIdAsync(entity.IdRole, ct);
    }

    public async Task<bool> UpdateAsync(int id, UpdateRoleDTO dto, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        _mapper.Map(dto, entity);
        if (!await ValidateAsync(entity, ct)) return false;
        _repository.Update(entity);
        await _repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> PatchAsync(int id, PatchRoleDTO dto, CancellationToken ct)
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

    private async Task<bool> ValidateAsync(Role entity, CancellationToken ct)
    {
        var existing = await _repository.FindByRoleNameAsync(entity.RoleName, ct);
        if (existing is not null && existing.IdRole != entity.IdRole)
            throw new InvalidOperationException("Значение RoleName уже используется.");
        if (string.IsNullOrWhiteSpace(entity.RoleName) || entity.RoleName.Length > 50)
            throw new InvalidOperationException("Поле RoleName обязательно и должно содержать не более 50 символов.");

        return true;
    }
}
