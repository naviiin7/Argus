using MediatR;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Interfaces;
using ShiftLess.Application.Features.Chat.Responses;
using ChatMessageEntity = ShiftLess.Domain.Entities.ChatMessage;

namespace ShiftLess.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandHandler
    : IRequestHandler<SendMessageCommand, ChatMessageResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public SendMessageCommandHandler(
        ITaskRepository taskRepository,
        IChatRepository chatRepository,
        IUserRepository userRepository,
        IRealtimeNotifier realtimeNotifier)
    {
        _taskRepository = taskRepository;
        _chatRepository = chatRepository;
        _userRepository = userRepository;
        _realtimeNotifier = realtimeNotifier;
    }

    public async Task<ChatMessageResponse> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId);

        if (task is null)
            throw new NotFoundException("Task not found.");

        await EnsureParticipant(task.Id, task.ShopkeeperId, request.SenderId, request.Role);

        if (string.IsNullOrWhiteSpace(request.Request.Message))
            throw new BadRequestException("Message cannot be empty.");

        var sender = await _userRepository.GetByIdAsync(request.SenderId);

        var message = new ChatMessageEntity
        {
            TaskRequestId = task.Id,
            SenderId = request.SenderId,
            Message = request.Request.Message.Trim(),
            SentAt = DateTime.UtcNow
        };

        await _chatRepository.AddMessageAsync(message);
        await _chatRepository.SaveChangesAsync();

        var response = new ChatMessageResponse
        {
            MessageId = message.Id,
            SenderId = message.SenderId,
            SenderName = sender?.FullName ?? "Unknown",
            Message = message.Message,
            SentAt = message.SentAt
        };

        // Live update for anyone who has this task's chat open.
        await _realtimeNotifier.SendChatMessageAsync(task.Id, response);

        // Toast / browser notification for everyone else in the chat,
        // even when they don't have it open.
        await NotifyOtherParticipantsAsync(task.Id, task.Title, task.ShopkeeperId, response);

        return response;
    }

    private async Task NotifyOtherParticipantsAsync(
        int taskId,
        string taskTitle,
        int shopkeeperId,
        ChatMessageResponse message)
    {
        // A task can have several accepted workers, so notify all of them
        // plus the manager.
        var recipients = new HashSet<int> { shopkeeperId };

        foreach (var workerId in await _taskRepository.GetAcceptedWorkerIdsAsync(taskId))
            recipients.Add(workerId);

        // Never notify the person who just sent the message.
        recipients.Remove(message.SenderId);

        foreach (var recipientId in recipients)
        {
            await _realtimeNotifier.NotifyUserAsync(
                recipientId,
                "ChatMessage",
                new
                {
                    taskId,
                    taskTitle,
                    senderName = message.SenderName,
                    text = message.Message
                });
        }
    }

    private async Task EnsureParticipant(
        int taskId,
        int shopkeeperId,
        int senderId,
        string role)
    {
        if (role == "Admin")
            return;

        if (role == "Manager" && shopkeeperId == senderId)
            return;

        var acceptedApplication =
            await _taskRepository.GetAcceptedApplicationAsync(taskId, senderId);

        if (acceptedApplication is not null)
            return;

        throw new ForbiddenException(
            "Only the task's manager and its accepted worker can use this chat.");
    }
}