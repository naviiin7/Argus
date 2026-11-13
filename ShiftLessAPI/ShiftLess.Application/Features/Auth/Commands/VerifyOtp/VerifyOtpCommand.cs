using MediatR;
using ShiftLess.Application.Features.Auth.Responses;

namespace ShiftLess.Application.Features.Auth.Commands.VerifyOtp;

public record VerifyOtpCommand(
    string Email,
    string Purpose,
    string Code,
    bool Consume)
    : IRequest<VerifyOtpResponse>;