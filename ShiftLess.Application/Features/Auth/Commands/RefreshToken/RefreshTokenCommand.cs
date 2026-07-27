using MediatR;
using ShiftLess.Application.Features.Auth.DTOs;
using ShiftLess.Application.Features.Auth.Responses;

namespace ShiftLess.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    RefreshTokenRequest Request)
    : IRequest<LoginResponse>;