using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShiftLess.Domain.Entities;
using ShiftLess.Domain.Enums;
using ShiftLess.Persistence.Context;
namespace ShiftLess.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        // Ensure at least one Admin always exists
        if (!await context.Users.AnyAsync(x => x.Role == UserRole.Admin))
        {
            var admin = new User
            {
                FullName = "Super Admin",
                Email = "admin@shiftless.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin
            };

            context.Users.Add(admin);

            await context.SaveChangesAsync();
        }
    }
}