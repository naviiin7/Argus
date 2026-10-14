namespace ShiftLess.Application.Interfaces;

public interface IRealtimeNotifier
{
    Task SendChatMessageAsync(int taskId, object message);

    Task NotifyUserAsync(int userId, string eventType, object payload);
}