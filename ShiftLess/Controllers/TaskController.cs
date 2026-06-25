using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftLess.Application.Features.Tasks.Commands.AcceptApplication;
using ShiftLess.Application.Features.Tasks.Commands.ApplyTask;
using ShiftLess.Application.Features.Tasks.Commands.CreateTask;
using ShiftLess.Application.Features.Tasks.Commands.RejectApplication;
using ShiftLess.Application.Features.Tasks.DTOs;
using ShiftLess.Application.Features.Tasks.Queries.GetApplicants;
using ShiftLess.Application.Features.Tasks.Queries.GetMyCreatedTasks;
using ShiftLess.Application.Features.Tasks.Queries.GetMyTasks;
using ShiftLess.Application.Features.Tasks.Queries.GetOpenTasks;
using ShiftLess.Application.Features.Tasks.Queries.GetTaskDetails;
using ShiftLess.Application.Features.Tasks.Commands.UpdateTask;
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

    // =====================================================
    // PUBLIC TASKS
    // =====================================================

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetOpenTasks()
    {
        var result =
            await _mediator.Send(
                new GetOpenTasksQuery());

        return Ok(result);
    }

    [Authorize(Roles = "Client,Manager")]
    [HttpGet("my-tasks")]
    public async Task<IActionResult> GetMyTasks()
    {
        var workerId = int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var result =
            await _mediator.Send(
                new GetMyTasksQuery(workerId));

        return Ok(result);
    }

    // =====================================================
    // WORKER APPLICATIONS
    // =====================================================

    [Authorize(Roles = "Client,Manager")]
    [HttpPost("{id}/apply")]
    public async Task<IActionResult> Apply(int id)
    {
        var workerId = int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var result =
            await _mediator.Send(
                new ApplyTaskCommand(
                    id,
                    workerId));

        return Ok(result);
    }

    // =====================================================
    // MANAGER TASKS
    // =====================================================

    [Authorize(Roles = "Manager,Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateTask(
        CreateTaskRequest request)
    {
        var managerId = int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var result =
            await _mediator.Send(
                new CreateTaskCommand(
                    managerId,
                    request));

        return Ok(result);
    }

    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("created")]
    public async Task<IActionResult> GetMyCreatedTasks()
    {
        var managerId = int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var result =
            await _mediator.Send(
                new GetMyCreatedTasksQuery(
                    managerId));

        return Ok(result);
    }

    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetTaskDetails(
        int id)
    {
        var managerId = int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var result =
            await _mediator.Send(
                new GetTaskDetailsQuery(
                    id,
                    managerId));

        return Ok(result);
    }

    // =====================================================
    // MANAGER APPLICATIONS
    // =====================================================

    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("{id}/applicants")]
    public async Task<IActionResult> GetApplicants(
        int id)
    {
        var result =
            await _mediator.Send(
                new GetApplicantsQuery(id));

        return Ok(result);
    }

    [Authorize(Roles = "Manager,Admin")]
    [HttpPost("{taskId}/applications/{applicationId}/accept")]
    public async Task<IActionResult> AcceptApplication(
        int taskId,
        int applicationId)
    {
        var shopkeeperId = int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var result =
            await _mediator.Send(
                new AcceptApplicationCommand(
                    taskId,
                    applicationId,
                    shopkeeperId));

        return Ok(result);
    }

    [Authorize(Roles = "Manager,Admin")]
    [HttpPost("{taskId}/applications/{applicationId}/reject")]
    public async Task<IActionResult> RejectApplication(
        int taskId,
        int applicationId)
    {
        var shopkeeperId = int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var result =
            await _mediator.Send(
                new RejectApplicationCommand(
                    taskId,
                    applicationId,
                    shopkeeperId));

        return Ok(result);
    }


    [Authorize(Roles = "Manager,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(
    int id,
    UpdateTaskRequest request)
    {
        var managerId = int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var result =
            await _mediator.Send(
                new UpdateTaskCommand(
                    id,
                    managerId,
                    request));

        return Ok(result);
    }
    // =====================================================
    // FUTURE ENDPOINTS
    // =====================================================

    // GET    /api/tasks/{id}
    // PUT    /api/tasks/{id}
    // DELETE /api/tasks/{id}

    // POST   /api/tasks/{taskId}/complete
    // POST   /api/tasks/{taskId}/leave

    // SignalR Chat
    // GET    /api/tasks/{taskId}/chat

    // Task Members
    // GET    /api/tasks/{taskId}/members
}