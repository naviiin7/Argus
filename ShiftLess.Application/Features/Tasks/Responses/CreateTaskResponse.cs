using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Tasks.Responses;

public class CreateTaskResponse
{
    public int TaskId { get; set; }

    public string Message { get; set; } = string.Empty;
}