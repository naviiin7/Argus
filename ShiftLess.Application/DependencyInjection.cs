using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ShiftLess.Application.Assembly;
using ShiftLess.Application.Common.Behaviors;

namespace ShiftLess.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(ApplicationAssemblyMarker).Assembly);
        });

        services.AddValidatorsFromAssembly(
            typeof(ApplicationAssemblyMarker).Assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        return services;
    }
}