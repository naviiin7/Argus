using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShiftLess.Application.Interfaces;
using ShiftLess.Infrastructure.Authentication;
using ShiftLess.Infrastructure.Payments;

namespace ShiftLess.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(
            configuration.GetSection("Jwt"));

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();

        services.Configure<RazorpaySettings>(
    configuration.GetSection("Razorpay"));

        services.AddHttpClient<IPaymentGateway, RazorpayGateway>();

        return services;
    }
}