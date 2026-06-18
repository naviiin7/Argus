using System;
using System.Collections.Generic;
using System.Text;

using MediatR;
using ShiftLess.Application.Features.Auth.DTOs;
using ShiftLess.Application.Features.Auth.Responses;

namespace ShiftLess.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    LoginRequest Request)
    : IRequest<LoginResponse>;