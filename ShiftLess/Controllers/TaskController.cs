using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftLess.Application.Features.Tasks.Commands.CreateTask;
using ShiftLess.Application.Features.Tasks.DTOs;
using System.Security.Claims;

namespace ShiftLessAPI.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize (Roles = "Manager,Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateTask(
        CreateTaskRequest request)
    {
        var shopkeeperId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(
            new CreateTaskCommand(
                shopkeeperId,
                request));

        return Ok(result);
    }
}