using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShiftLess.Application.Features.Auth.Commands.Login;
using ShiftLess.Application.Features.Auth.Commands.Register;
using ShiftLess.Application.Features.Auth.Commands.RefreshToken;
using ShiftLess.Application.Features.Auth.Commands.SendOtp;
using ShiftLess.Application.Features.Auth.Commands.VerifyOtp;
using ShiftLess.Application.Features.Auth.DTOs;
using ShiftLess.Application.Features.Auth.Otp;
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

    [HttpPost("register/send-otp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SendRegistrationOtp(SendOtpRequest request)
    {
        var result = await _mediator.Send(
            new SendOtpCommand(request.Email, OtpPurpose.Registration));

        return this.ApiOk(result, "Verification code sent.");
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // 1. Check the code without burning it, so a failed registration
        //    (validation error, race on the email) doesn't force another email.
        await _mediator.Send(new VerifyOtpCommand(
            request.Email, OtpPurpose.Registration, request.OtpCode, Consume: false));

        var result = await _mediator.Send(new RegisterCommand(request));

        // 2. Registration succeeded, so burn the code.
        await _mediator.Send(new VerifyOtpCommand(
            request.Email, OtpPurpose.Registration, request.OtpCode, Consume: true));

        return this.ApiOk(result, "Registration successful.");
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _mediator.Send(
            new LoginCommand(request));

        return this.ApiOk(result, "Login successful.");
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var result = await _mediator.Send(
            new RefreshTokenCommand(request));

        return this.ApiOk(result, "Token refreshed successfully.");
    }
}