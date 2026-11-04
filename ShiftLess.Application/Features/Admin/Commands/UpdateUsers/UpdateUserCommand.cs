using MediatR;
using ShiftLess.Application.Features.Admin.DTOs;
using ShiftLess.Application.Features.Admin.Responses;

namespace ShiftLess.Application.Features.Admin.Commands.UpdateUsers;

public record UpdateUserCommand(
    int AdminId,
    int UserId,
    UpdateUserRequest Request) : IRequest<AdminActionResponse>;