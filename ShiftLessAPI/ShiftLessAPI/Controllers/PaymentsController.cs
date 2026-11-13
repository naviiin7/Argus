using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftLess.Application.Features.Payments.Commands.CreateOrder;
using ShiftLess.Application.Features.Payments.Commands.PayTask;
using ShiftLess.Application.Features.Payments.Commands.VerifyPayment;
using ShiftLess.Application.Features.Payments.DTOs;
using ShiftLessAPI.Extensions;
using System.Security.Claims;

namespace ShiftLessAPI.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize(Roles = "Manager,Admin")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private string CurrentRole =>
        User.FindFirstValue(ClaimTypes.Role)!;

    /// <summary>
    /// Demo card payment (mock processor). No real money moves.
    /// </summary>
    [HttpPost("tasks/{taskId}/pay")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Pay(int taskId, PayTaskRequest request)
    {
        var result = await _mediator.Send(
            new PayTaskCommand(taskId, CurrentUserId, CurrentRole, request));

        return this.ApiOk(result, "Payment successful.");
    }

    // ---- Razorpay flow (parked, unused by the UI for now) ----

    [HttpPost("tasks/{taskId}/order")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrder(int taskId)
    {
        var result = await _mediator.Send(
            new CreateOrderCommand(taskId, CurrentUserId, CurrentRole));

        return this.ApiOk(result, "Order created successfully.");
    }

    [HttpPost("verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Verify(VerifyPaymentRequest request)
    {
        var result = await _mediator.Send(
            new VerifyPaymentCommand(CurrentUserId, CurrentRole, request));

        return this.ApiOk(result, "Payment verified.");
    }
}