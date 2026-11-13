using MediatR;
using ShiftLess.Application.Features.Auth.Responses;

namespace ShiftLess.Application.Features.Auth.Commands.SendOtp;

public record SendOtpCommand(
    string Email,
    string Purpose)
    : IRequest<SendOtpResponse>;