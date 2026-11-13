using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftLess.Application.Features.Chat.Commands.SendMessage;
using ShiftLess.Application.Features.Chat.DTOs;
using ShiftLess.Application.Features.Chat.Queries.GetMessages;
using ShiftLessAPI.Extensions;
using System.Security.Claims;

namespace ShiftLessAPI.Controllers;

[ApiController]
[Route("api/tasks/{taskId}/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private string CurrentRole =>
        User.FindFirstValue(ClaimTypes.Role)!;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMessages(int taskId)
    {
        var result = await _mediator.Send(
            new GetMessagesQuery(taskId, CurrentUserId, CurrentRole));

        return this.ApiOk(result, "Messages retrieved successfully.");
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SendMessage(
        int taskId,
        SendChatMessageRequest request)
    {
        var result = await _mediator.Send(
            new SendMessageCommand(taskId, CurrentUserId, CurrentRole, request));

        return this.ApiOk(result, "Message sent successfully.");
    }
}