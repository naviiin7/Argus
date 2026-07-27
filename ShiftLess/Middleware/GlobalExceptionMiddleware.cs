using ShiftLess.Application.Common.Exceptions;
using ShiftLessAPI.Models;
using System.Text.Json;

namespace ShiftLessAPI.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleException(context, ex);
        }
    }

    private static async Task HandleException(
        HttpContext context,
        Exception exception)
    {
        var response = new ApiErrorResponse
        {
            Success = false,
            Message = exception.Message,
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case ValidationException validationException:

                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Errors = validationException.Errors.ToList();

                break;

            case NotFoundException:

                response.StatusCode = StatusCodes.Status404NotFound;

                break;

            case ForbiddenException:

                response.StatusCode = StatusCodes.Status403Forbidden;

                break;

            case UnauthorizedAccessException:

                response.StatusCode = StatusCodes.Status401Unauthorized;

                break;

            case BadRequestException:

                response.StatusCode = StatusCodes.Status400BadRequest;

                break;

            default:

                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An unexpected error occurred.";

                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}