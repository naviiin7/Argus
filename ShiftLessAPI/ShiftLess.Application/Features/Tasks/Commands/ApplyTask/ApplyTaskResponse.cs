using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Tasks.Commands.ApplyTask;

public class ApplyTaskResponse
{
    public int ApplicationId { get; set; }

    public string Message { get; set; } = string.Empty;
}