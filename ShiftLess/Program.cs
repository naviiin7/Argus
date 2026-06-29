using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShiftLess.Application.Assembly;
using ShiftLess.Infrastructure;
using ShiftLess.Persistence;
using ShiftLess.Persistence.Seed;
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

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(ApplicationAssemblyMarker).Assembly);
        });

        // =========================================
        // INFRASTRUCTURE + PERSISTENCE
        // =========================================

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddPersistence(builder.Configuration);

        // =========================================
        // AUTHENTICATION
        // =========================================

        var jwtKey = builder.Configuration["Jwt:Key"];

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer =
                            builder.Configuration["Jwt:Issuer"],

                        ValidAudience =
                            builder.Configuration["Jwt:Audience"],

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtKey!))
                    };
            });

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
        await DbSeeder.SeedAsync(app.Services);

        // =========================================
        // MIDDLEWARE PIPELINE
        // =========================================

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}