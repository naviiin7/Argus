using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShiftLess.Application.Interfaces;
using System.Security.Claims;

namespace ShiftLessAPI.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        ITaskRepository taskRepository,
        ILogger<ChatHub> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task JoinTaskGroup(int taskId)
    {
        if (!int.TryParse(Context.UserIdentifier, out var userId))
        {
            throw new HubException("Not authenticated.");
        }

        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (!await CanAccessTaskChatAsync(taskId, userId, role))
        {
            _logger.LogWarning(
                "Denied chat group join: user={UserId} task={TaskId}", userId, taskId);

            throw new HubException(
                "Only the task's manager and its accepted worker can join this chat.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, TaskGroupName(taskId));
    }

    public async Task LeaveTaskGroup(int taskId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, TaskGroupName(taskId));
    }

    public static string TaskGroupName(int taskId) => $"task-{taskId}";

    public static string UserGroupName(int userId) => $"user-{userId}";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;

        _logger.LogInformation(
            "Hub connected: user={UserId} connection={ConnectionId}",
            userId ?? "(anonymous)", Context.ConnectionId);

        if (userId is not null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroupName(int.Parse(userId)));
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation(
            "Hub disconnected: user={UserId} connection={ConnectionId}",
            Context.UserIdentifier ?? "(anonymous)", Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }

    
    private async Task<bool> CanAccessTaskChatAsync(int taskId, int userId, string? role)
    {
        if (role == "Admin")
            return true;

        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task is null)
            return false;

        if (task.ShopkeeperId == userId)
            return true;

        var accepted = await _taskRepository.GetAcceptedApplicationAsync(taskId, userId);

        return accepted is not null;
    }
}