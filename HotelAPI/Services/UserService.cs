using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using HotelAPI.DTOs;
using HotelAPI.Models;
using HotelAPI.Repositories;

namespace HotelAPI.Services;

public class UserService : IUserService
{
    private readonly PasswordHasher<User> _hasher = new();
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly IRoleRepository _roleRepository;

    public UserService(IUserRepository repository, IMapper mapper, IRoleRepository roleRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _roleRepository = roleRepository;
    }

    public async Task<(List<UserDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            throw new InvalidOperationException("Некорректные параметры пагинации.");
        var entities = await _repository.GetAllAsync(page, pageSize, ct);
        var total = await _repository.CountAsync(ct);
        return (_mapper.Map<List<UserDTO>>(entities), total);
    }

    public async Task<UserDTO?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<UserDTO>(entity);
    }

    public async Task<UserDTO?> CreateAsync(CreateUserDTO dto, CancellationToken ct)
    {
        var entity = _mapper.Map<User>(dto);
        entity.PasswordHash = Hash(entity, dto.UserPassword);
        if (!await ValidateAsync(entity, ct)) return null;
        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
        return await GetByIdAsync(entity.IdUser, ct);
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserDTO dto, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        _mapper.Map(dto, entity);
        if (dto.UserPassword is not null) entity.PasswordHash = Hash(entity, dto.UserPassword);
        if (!await ValidateAsync(entity, ct)) return false;
        _repository.Update(entity);
        await _repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> PatchAsync(int id, PatchUserDTO dto, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;
        _mapper.Map(dto, entity);
        if (dto.UserPassword is not null) entity.PasswordHash = Hash(entity, dto.UserPassword);
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

    private string Hash(User user, string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8 || password.Length > 128)
            throw new InvalidOperationException("Пароль должен содержать от 8 до 128 символов.");
        return _hasher.HashPassword(user, password);
    }
    public async Task<User> RegisterAsync(string login, string password, CancellationToken ct)
    {
        login = login.Trim();
        if (login.Length < 3 || login.Length > 50) throw new InvalidOperationException("Логин: от 3 до 50 символов.");
        if (await _repository.FindByLoginAsync(login, ct) is not null) throw new InvalidOperationException("Логин уже занят.");
        var role = await _roleRepository.FindByRoleNameAsync("User", ct)
            ?? throw new InvalidOperationException("Сначала выполните инициализацию базы.");
        var user = new User { Login = login, RoleId = role.IdRole, Role = role, IsActive = true };
        user.PasswordHash = Hash(user, password);
        await _repository.AddAsync(user, ct);
        try { await _repository.SaveChangesAsync(ct); }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
        { throw new InvalidOperationException("Логин уже занят."); }
        return user;
    }
    public async Task<User?> ValidateCredentialsAsync(string login, string password, CancellationToken ct)
    {
        var user = await _repository.FindByLoginAsync(login.Trim(), ct);
        if (user is null || !user.IsActive) return null;
        PasswordVerificationResult result;
        try { result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password); }
        catch (FormatException) { return null; }
        if (result == PasswordVerificationResult.Failed) return null;
        if (result == PasswordVerificationResult.SuccessRehashNeeded) {
            user.PasswordHash = _hasher.HashPassword(user, password);
            await _repository.SaveChangesAsync(ct);
        }
        return user;
    }
    private async Task<bool> ValidateAsync(User entity, CancellationToken ct)
    {
        var existing = await _repository.FindByLoginAsync(entity.Login, ct);
        if (existing is not null && existing.IdUser != entity.IdUser)
            throw new InvalidOperationException("Значение Login уже используется.");
        if (string.IsNullOrWhiteSpace(entity.Login) || entity.Login.Length > 50)
            throw new InvalidOperationException("Поле Login обязательно и должно содержать не более 50 символов.");
        if (string.IsNullOrWhiteSpace(entity.PasswordHash) || entity.PasswordHash.Length > 500)
            throw new InvalidOperationException("Хеш пароля отсутствует или слишком длинный.");
        if (entity.UserName?.Length > 100) throw new InvalidOperationException("Поле UserName: не более 100 символов.");
        if (await _roleRepository.GetByIdAsync(entity.RoleId, ct) is null) return false;
        return true;
    }
}
