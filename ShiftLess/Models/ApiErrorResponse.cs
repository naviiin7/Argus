namespace ShiftLessAPI.Models;

public class ApiErrorResponse
{
    public bool Success { get; set; }

    public int StatusCode { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<string>? Errors { get; set; }

    public string? TraceId { get; set; }
}