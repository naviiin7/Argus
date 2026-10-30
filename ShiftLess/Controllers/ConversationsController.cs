using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftLess.Application.Features.Chat.Queries.GetConversations;
using ShiftLessAPI.Extensions;
using System.Security.Claims;

namespace ShiftLessAPI.Controllers;

[ApiController]
[Route("api/chat/conversations")]
[Authorize(Roles = "Manager,Admin")]
public class ConversationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConversationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversations()
    {
        var result = await _mediator.Send(
            new GetConversationsQuery(CurrentUserId));

        return this.ApiOk(result, "Conversations retrieved successfully.");
    }
}