using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ShiftLessAPI.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinTaskGroup(int taskId)
    {
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
}