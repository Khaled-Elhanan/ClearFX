using ClearFX.Domain.Entities;
using ClearFX.Domain.Enums;
using Microsoft.EntityFrameworkCore;
namespace ClearFX.Infrastructure.Persistence;

public class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync(u => u.Email == "admin@clearfx.com"))
            return;

        var admin = new User
        {
            FullName     = "System Admin",
            Email        = "admin@clearfx.com",
            Role         = UserRole.Admin,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123456"),
            IsActive     = true
        };

        await db.Users.AddAsync(admin);
        await db.SaveChangesAsync();
    }
}
