using Microsoft.EntityFrameworkCore.Storage;
using HotelAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace HotelAPI.Data;
// Explicit one-time setup, never invoked during a normal API startup.
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(HotelContext db)
    {
        if (db.Database.GetDbConnection().Database != "HOTEL")
            throw new InvalidOperationException("Инициализация разрешена только для HOTEL.");
        await using var transaction = await db.Database.BeginTransactionAsync();
        var connection = db.Database.GetDbConnection();
        await using var command = connection.CreateCommand();
        command.Transaction = transaction.GetDbTransaction();
        command.CommandText = "SELECT CASE WHEN COL_LENGTH('dbo.users','user_password') IS NOT NULL THEN 1 ELSE 0 END";
        bool legacy = Convert.ToInt32(await command.ExecuteScalarAsync()) == 1;
        if (legacy) {
            await db.Database.ExecuteSqlRawAsync("EXEC sp_rename 'dbo.users.user_password', 'password_hash', 'COLUMN'; ALTER TABLE dbo.users ALTER COLUMN password_hash varchar(500) NOT NULL;");
            var hasher = new PasswordHasher<User>();
            foreach (var user in await db.Users.ToListAsync())
                user.PasswordHash = hasher.HashPassword(user, user.PasswordHash);
            await db.SaveChangesAsync();
        }
        // Preserve existing role IDs and user relationships.
        foreach (var pair in new[] { ("Админ", "Admin"), ("Клиент", "User") })
        {
            var existing = await db.Roles.SingleOrDefaultAsync(x => x.RoleName == pair.Item1);
            if (existing != null && !await db.Roles.AnyAsync(x => x.RoleName == pair.Item2))
                existing.RoleName = pair.Item2;
        }
        await db.SaveChangesAsync();
        foreach (var roleName in new[] { "Admin", "User" })
            if (!await db.Roles.AnyAsync(x => x.RoleName == roleName))
                db.Roles.Add(new Role { RoleName = roleName });
        await db.SaveChangesAsync();
        var passwordHasher = new PasswordHasher<User>();
        foreach (var entry in new[] {
            ("demo_admin", "Admin", true), ("demo_user1", "User", true),
            ("demo_user2", "User", true), ("demo_inactive", "User", false) })
        {
            if (await db.Users.AnyAsync(x => x.Login == entry.Item1)) continue;
            var role = await db.Roles.SingleAsync(x => x.RoleName == entry.Item2);
            var user = new User { Login = entry.Item1, Role = role, RoleId = role.IdRole,
                UserName = entry.Item1, IsActive = entry.Item3 };
            user.PasswordHash = passwordHasher.HashPassword(user, "Practice11!");
            db.Users.Add(user);
        }
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        Console.WriteLine("HOTEL initialized. Demo accounts: demo_admin, demo_user1, demo_user2, demo_inactive. Password: Practice11!");
    }
}

