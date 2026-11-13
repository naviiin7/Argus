using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftLess.Application.Features.Admin.Commands.DeleteUser;
using ShiftLess.Application.Features.Admin.Commands.SetUserActive;
using ShiftLess.Application.Features.Admin.Commands.UpdateUsers;
using ShiftLess.Application.Features.Admin.DTOs;
using ShiftLess.Application.Features.Admin.Queries.GetAdminStats;
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

    // ---------------------------------------------------------
    // Statistics
    // ---------------------------------------------------------

    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        var result = await _mediator.Send(new GetAdminStatsQuery());

        return this.ApiOk(result, "Statistics retrieved successfully.");
    }

    // ---------------------------------------------------------
    // Users
    // ---------------------------------------------------------

    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());

        return this.ApiOk(result, "Users retrieved successfully.");
    }

    [HttpPut("users/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
    {
        var result = await _mediator.Send(
            new UpdateUserCommand(CurrentUserId, id, request));

        return this.ApiOk(result, "User updated successfully.");
    }

    [HttpPost("users/{id}/block")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BlockUser(int id)
    {
        var result = await _mediator.Send(
            new SetUserActiveCommand(CurrentUserId, id, false));

        return this.ApiOk(result, "User blocked.");
    }

    [HttpPost("users/{id}/unblock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnblockUser(int id)
    {
        var result = await _mediator.Send(
            new SetUserActiveCommand(CurrentUserId, id, true));

        return this.ApiOk(result, "User unblocked.");
    }

    [HttpDelete("users/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _mediator.Send(
            new DeleteUserCommand(CurrentUserId, id));

        return this.ApiOk(result, "User deleted.");
    }

    // ---------------------------------------------------------
    // Tasks
    // ---------------------------------------------------------

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