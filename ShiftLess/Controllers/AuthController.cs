using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShiftLess.Application.Features.Auth.Commands.Login;
using ShiftLess.Application.Features.Auth.Commands.Register;
using ShiftLess.Application.Features.Auth.DTOs;
using ShiftLessAPI.Extensions;

namespace ShiftLessAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _mediator.Send(
            new RegisterCommand(request));

        return this.ApiOk(
            result,
            "Registration successful.");
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _mediator.Send(
            new LoginCommand(request));

        return this.ApiOk(
            result,
            "Login successful.");
    }
}