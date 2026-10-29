using Microsoft.AspNetCore.Mvc;
using ShiftLessAPI.Models;

namespace ShiftLessAPI.Extensions;

public static class ControllerExtensions
{
    public static OkObjectResult ApiOk<T>(
        this ControllerBase controller,
        T data,
        string message = "Success")
    {
        return controller.Ok(ApiResponse<T>.Ok(data, message));
    }
}