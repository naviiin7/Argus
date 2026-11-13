using MediatR;
using ShiftLess.Application.Features.Auth.DTOs;
using ShiftLess.Application.Features.Auth.Responses;

namespace ShiftLess.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    RegisterRequest Request)
    : IRequest<RegisterResponse>;