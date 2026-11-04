using MediatR;
using ShiftLess.Application.Features.Admin.Responses;

namespace ShiftLess.Application.Features.Admin.Commands.DeleteUser;

public record DeleteUserCommand(
    int AdminId,
    int UserId) : IRequest<AdminActionResponse>;