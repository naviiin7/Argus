using Microsoft.AspNetCore.SignalR;
using ShiftLess.Application.Interfaces;

namespace ShiftLessAPI.Hubs;

public class SignalRNotifier : IRealtimeNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<SignalRNotifier> _logger;

    public SignalRNotifier(
        IHubContext<ChatHub> hubContext,
        ILogger<SignalRNotifier> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendChatMessageAsync(int taskId, object message)
    {
        await _hubContext.Clients
            .Group(ChatHub.TaskGroupName(taskId))
            .SendAsync("ReceiveMessage", message);
    }

    public async Task NotifyUserAsync(int userId, string eventType, object payload)
    {
        _logger.LogInformation(
            "Notify user={UserId} type={Type}", userId, eventType);

        // userId travels with the notification so the browser can verify
        // it is really meant for the person logged in there.
        await _hubContext.Clients
            .Group(ChatHub.UserGroupName(userId))
            .SendAsync("Notification", new { type = eventType, userId, payload });
    }
}