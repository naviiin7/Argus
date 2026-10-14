using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ShiftLessAPI.Hubs;

[Authorize]
public class ChatHub : Hub
{
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

        if (userId is not null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroupName(int.Parse(userId)));
        }

        await base.OnConnectedAsync();
    }
}