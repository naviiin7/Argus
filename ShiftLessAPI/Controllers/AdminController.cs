using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftLess.Application.Features.Admin.Queries.GetAllTasks;
using ShiftLess.Application.Features.Admin.Queries.GetAllUsers;
using ShiftLess.Application.Features.Tasks.Commands.DeleteTask;
using ShiftLessAPI.Extensions;
using System.Security.Claims;

namespace ShiftLessAPI.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());

        return this.ApiOk(result, "Users retrieved successfully.");
    }

    [HttpGet("tasks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTasks()
    {
        var result = await _mediator.Send(new GetAllTasksQuery());

        return this.ApiOk(result, "Tasks retrieved successfully.");
    }

    [HttpDelete("tasks/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var result = await _mediator.Send(
            new DeleteTaskCommand(id, CurrentUserId, "Admin"));

        return this.ApiOk(result, "Task deleted successfully.");
    }
}