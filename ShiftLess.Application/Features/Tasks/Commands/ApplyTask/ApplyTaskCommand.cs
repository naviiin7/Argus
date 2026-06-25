using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace ShiftLess.Application.Features.Tasks.Commands.ApplyTask;

public record ApplyTaskCommand(
    int TaskId,
    int WorkerId)
    : IRequest<ApplyTaskResponse>;
