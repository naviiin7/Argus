using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShiftLess.Domain.Entities;
using ShiftLess.Domain.Enums;
using ShiftLess.Persistence.Context;

namespace ShiftLess.Persistence.Seed;

public static class DbSeeder
{
    // The well-known account an old version of this app created. Its
    // password is public, so it is retired outside local development.
    private const string LegacyAdminEmail = "admin@shiftless.com";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var configuration = scope.ServiceProvider
            .GetRequiredService<IConfiguration>();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DbSeeder");

        await context.Database.MigrateAsync();

        var isDevelopment = string.Equals(
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            "Development",
            StringComparison.OrdinalIgnoreCase);

        var configuredEmail = configuration["Admin:Email"]?.Trim();
        var configuredPassword = configuration["Admin:Password"];

        var configured =
            !string.IsNullOrWhiteSpace(configuredEmail) &&
            !string.IsNullOrWhiteSpace(configuredPassword);

        // Local development convenience only.
        if (!configured && isDevelopment)
        {
            configuredEmail = LegacyAdminEmail;
            configuredPassword = "Admin@123";
            configured = true;
        }

        string? email = configured ? configuredEmail!.ToLowerInvariant() : null;

        // 1. Create (or refresh) the configured admin.
        if (configured)
        {
            var admin = await context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email);

            if (admin is null)
            {
                context.Users.Add(new User
                {
                    FullName = "Super Admin",
                    Email = configuredEmail!,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(configuredPassword!),
                    Role = UserRole.Admin
                });

                logger.LogInformation(
                    "Created admin account for {Email}.", configuredEmail);
            }
            else if (admin.Role != UserRole.Admin)
            {
                // Never silently promote an ordinary account.
                logger.LogWarning(
                    "{Email} already belongs to a non-admin account and was NOT promoted. " +
                    "Use a different Admin__Email.", configuredEmail);
            }
            else
            {
                admin.IsActive = true;

                // One-off: set Admin__ResetPassword=true for a single deploy
                // to overwrite this admin's password, then remove it.
                if (bool.TryParse(configuration["Admin:ResetPassword"], out var reset) && reset)
                {
                    admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(configuredPassword!);

                    logger.LogInformation(
                        "Reset the password for admin {Email}.", configuredEmail);
                }
            }
        }
        else
        {
            logger.LogWarning(
                "Admin__Email / Admin__Password are not configured, so no admin account was created.");
        }

        // 2. Retire the public default admin outside local development.
        if (!isDevelopment && email != LegacyAdminEmail)
        {
            var legacy = await context.Users.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == LegacyAdminEmail &&
                u.Role == UserRole.Admin);

            if (legacy is not null && legacy.IsActive)
            {
                legacy.IsActive = false;

                legacy.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));

                logger.LogInformation(
                    "Retired the default admin account {Email}.", LegacyAdminEmail);
            }
        }

        await context.SaveChangesAsync();
    }
}