using MediatR;
using ShiftLess.Application.Features.Admin.Responses;

namespace ShiftLess.Application.Features.Admin.Commands.SetUserActive;

public record SetUserActiveCommand(
    int AdminId,
    int UserId,
    bool IsActive) : IRequest<AdminActionResponse>;