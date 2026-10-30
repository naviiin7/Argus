using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftLess.Application.Features.Tasks.Commands.AcceptApplication;
using ShiftLess.Application.Features.Tasks.Commands.ApplyTask;
using ShiftLess.Application.Features.Tasks.Commands.CompleteTask;
using ShiftLess.Application.Features.Tasks.Commands.CreateTask;
using ShiftLess.Application.Features.Tasks.Commands.DeleteTask;
using ShiftLess.Application.Features.Tasks.Commands.LeaveTask;
using ShiftLess.Application.Features.Tasks.Commands.RejectApplication;
using ShiftLess.Application.Features.Tasks.Commands.UpdateTask;
using ShiftLess.Application.Features.Tasks.DTOs;
using ShiftLess.Application.Features.Tasks.Queries.GetApplicants;
using ShiftLess.Application.Features.Tasks.Queries.GetMyCreatedTasks;
using ShiftLess.Application.Features.Tasks.Queries.GetMyTasks;
using ShiftLess.Application.Features.Tasks.Queries.GetOpenTasks;
using ShiftLess.Application.Features.Tasks.Queries.GetTaskDetails;
using ShiftLessAPI.Extensions;
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

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private string CurrentRole =>
        User.FindFirstValue(ClaimTypes.Role)!;

    // =====================================================
    // PUBLIC TASKS
    // =====================================================

    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOpenTasks()
    {
        var result = await _mediator.Send(
            new GetOpenTasksQuery(CurrentUserId));
        return this.ApiOk(
            result,
            "Tasks retrieved successfully.");
    }

  
    [Authorize(Roles = "Client,Manager")]
    [HttpGet("my-tasks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyTasks()
    {
        var result = await _mediator.Send(
            new GetMyTasksQuery(CurrentUserId));

        return this.ApiOk(
            result,
            "Tasks retrieved successfully.");
    }

    // =====================================================
    // WORKER APPLICATIONS
    // =====================================================

    [Authorize(Roles = "Client,Manager")]
    [HttpPost("{id}/apply")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Apply(int id)
    {
        var result = await _mediator.Send(
            new ApplyTaskCommand(id, CurrentUserId));

        return this.ApiOk(
            result,
            "Application submitted successfully.");
    }

  
    [Authorize(Roles = "Client,Manager")]
    [HttpPost("{taskId}/leave")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LeaveTask(int taskId)
    {
        var result = await _mediator.Send(
            new LeaveTaskCommand(taskId, CurrentUserId));

        return this.ApiOk(
            result,
            "Task left successfully.");
    }

    // =====================================================
    // MANAGER TASKS
    // =====================================================

    [Authorize(Roles = "Manager,Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateTask(CreateTaskRequest request)
    {
        var result = await _mediator.Send(
            new CreateTaskCommand(CurrentUserId, request));

        return this.ApiOk(
            result,
            "Task created successfully.");
    }

    /// <summary>
    /// Returns tasks created by the current manager.
    /// </summary>
    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("created")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyCreatedTasks()
    {
        var result = await _mediator.Send(
            new GetMyCreatedTasksQuery(CurrentUserId));

        return this.ApiOk(
            result,
            "Created tasks retrieved successfully.");
    }

    /// <summary>
    /// Returns task details including applicants.
    /// </summary>
    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("{id}/details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTaskDetails(int id)
    {
        var result = await _mediator.Send(
            new GetTaskDetailsQuery(
                id,
                CurrentUserId,
                CurrentRole));

        return this.ApiOk(
            result,
            "Task details retrieved successfully.");
    }

    
    [Authorize(Roles = "Manager,Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTask(
        int id,
        UpdateTaskRequest request)
    {
        var result = await _mediator.Send(
            new UpdateTaskCommand(
                id,
                CurrentUserId,
                CurrentRole,
                request));

        return this.ApiOk(
            result,
            "Task updated successfully.");
    }

  
    [Authorize(Roles = "Manager,Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var result = await _mediator.Send(
            new DeleteTaskCommand(
                id,
                CurrentUserId,
                CurrentRole));

        return this.ApiOk(
            result,
            "Task deleted successfully.");
    }


    [Authorize(Roles = "Manager,Admin")]
    [HttpPost("{id}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CompleteTask(int id)
    {
        var result = await _mediator.Send(
            new CompleteTaskCommand(
                id,
                CurrentUserId,
                CurrentRole));

        return this.ApiOk(
            result,
            "Task completed successfully.");
    }

    // =====================================================
    // MANAGER APPLICATIONS
    // =====================================================

    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("{id}/applicants")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplicants(int id)
    {
        var result = await _mediator.Send(
            new GetApplicantsQuery(id));

        return this.ApiOk(
            result,
            "Applicants retrieved successfully.");
    }

   
    [Authorize(Roles = "Manager,Admin")]
    [HttpPost("{taskId}/applications/{applicationId}/accept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AcceptApplication(
        int taskId,
        int applicationId)
    {
        var result = await _mediator.Send(
            new AcceptApplicationCommand(
                taskId,
                applicationId,
                CurrentUserId,
                CurrentRole));

        return this.ApiOk(
            result,
            "Worker accepted successfully.");
    }


    [Authorize(Roles = "Manager,Admin")]
    [HttpPost("{taskId}/applications/{applicationId}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectApplication(
        int taskId,
        int applicationId)
    {
        var result = await _mediator.Send(
            new RejectApplicationCommand(
                taskId,
                applicationId,
                CurrentUserId,
                CurrentRole));

        return this.ApiOk(
            result,
            "Application rejected successfully.");
    }
}