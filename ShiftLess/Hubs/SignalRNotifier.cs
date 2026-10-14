using Microsoft.AspNetCore.SignalR;
using ShiftLess.Application.Interfaces;

namespace ShiftLessAPI.Hubs;

public class SignalRNotifier : IRealtimeNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRNotifier(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendChatMessageAsync(int taskId, object message)
    {
        await _hubContext.Clients
            .Group(ChatHub.TaskGroupName(taskId))
            .SendAsync("ReceiveMessage", message);
    }

    public async Task NotifyUserAsync(int userId, string eventType, object payload)
    {
        await _hubContext.Clients
            .Group(ChatHub.UserGroupName(userId))
            .SendAsync("Notification", new { type = eventType, payload });
    }
}