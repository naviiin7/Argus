namespace ShiftLessAPI.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public static ApiResponse<T> Ok(
        T data,
        string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }
}