using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShiftLess.Persistence.Context;
using ShiftLess.Application.Interfaces;
using ShiftLess.Persistence.Repositories;


namespace ShiftLess.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<ITaskRepository, TaskRepository>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}