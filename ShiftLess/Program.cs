using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShiftLess.Application;
using ShiftLess.Application.Assembly;
using ShiftLess.Application.Interfaces;
using ShiftLess.Infrastructure;
using ShiftLess.Infrastructure.Email;
using ShiftLess.Infrastructure.Otp;
using ShiftLess.Infrastructure.Payments;
using ShiftLess.Persistence;
using ShiftLess.Persistence.Repositories;
using ShiftLess.Persistence.Seed;
using ShiftLessAPI.Hubs;
using ShiftLessAPI.Middleware;
using System.Text;

namespace ShiftLess;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // =========================================
        // CORE FRAMEWORK SERVICES
        // =========================================

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // =========================================
        // APPLICATION LAYER
        // =========================================

        builder.Services.AddApplication();

        // =========================================
        // INFRASTRUCTURE + PERSISTENCE
        // =========================================

        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddPersistence(builder.Configuration);

        // =========================================
        // REAL-TIME (SignalR)
        // =========================================

        builder.Services.AddSignalR();

        builder.Services.AddScoped<IRealtimeNotifier, SignalRNotifier>();

        // =========================================
        // OTP + EMAIL
        // =========================================

        builder.Services.Configure<OtpSettings>(
            builder.Configuration.GetSection("Otp"));

        builder.Services.AddSingleton<IOtpService, OtpService>();

        builder.Services.AddScoped<IOtpRepository, OtpRepository>();

        builder.Services.Configure<EmailSettings>(
            builder.Configuration.GetSection("Email"));

        // Demo mode (codes logged, not emailed) until Email:Host is configured.
        builder.Services.AddSingleton<IEmailSender>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<EmailSettings>>().Value;

            return string.IsNullOrWhiteSpace(settings.Host)
                ? ActivatorUtilities.CreateInstance<ConsoleEmailSender>(sp)
                : ActivatorUtilities.CreateInstance<SmtpEmailSender>(sp);
        });

        // =========================================
        // PAYMENTS (demo card processor)
        // =========================================

        builder.Services.AddSingleton<ICardPaymentProcessor, MockCardPaymentProcessor>();

        // =========================================
        // CORS
        // =========================================
        // Production runs same-origin behind the reverse proxy, so CORS is
        // not actually exercised there. Origins come from configuration
        // (Cors:Origins:0, Cors:Origins:1, ...); the default keeps
        // `ng serve` on localhost:4200 working for local development.

        var corsOrigins =
            builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
            ?? new[] { "http://localhost:4200" };

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Angular", policy =>
            {
                policy
                    .WithOrigins(corsOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // =========================================
        // AUTHENTICATION
        // =========================================

        var jwtKey = builder.Configuration["Jwt:Key"];
        var jwtIssuer = builder.Configuration["Jwt:Issuer"];
        var jwtAudience = builder.Configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
        {
            throw new InvalidOperationException(
                "Jwt:Key must be configured and at least 32 characters long.");
        }

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,

                    ValidateAudience = true,
                    ValidAudience = jwtAudience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // Browsers can't set headers on a WebSocket handshake, so
                // SignalR sends the JWT as a query-string parameter instead.
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorization();

        // =========================================
        // SWAGGER
        // =========================================

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ShiftLess API",
                Version = "v1"
            });

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "Enter: Bearer {your JWT token}"
                });

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference =
                                new OpenApiReference
                                {
                                    Type =
                                        ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                        },
                        Array.Empty<string>()
                    }
                });
        });

        // =========================================
        // BUILD APP
        // =========================================

        var app = builder.Build();

        app.UseMiddleware<GlobalExceptionMiddleware>();

        await DbSeeder.SeedAsync(app.Services);

        // =========================================
        // MIDDLEWARE PIPELINE
        // =========================================

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // No UseHttpsRedirection(): TLS is terminated by Caddy, and the
        // container only listens on plain HTTP inside the Docker network.

        app.UseCors("Angular");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<ChatHub>("/hubs/chat");

        app.Run();
    }
}