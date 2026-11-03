using MediatR;
using ShiftLess.Application.Features.Auth.DTOs;
using ShiftLess.Application.Features.Auth.Responses;

namespace ShiftLess.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(ResetPasswordRequest Request)
    : IRequest<ResetPasswordResponse>;