using MediatR;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Features.Chat.Responses;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Chat.Queries.GetMessages;

public class GetMessagesQueryHandler
    : IRequestHandler<GetMessagesQuery, List<ChatMessageResponse>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IChatRepository _chatRepository;

    public GetMessagesQueryHandler(
        ITaskRepository taskRepository,
        IChatRepository chatRepository)
    {
        _taskRepository = taskRepository;
        _chatRepository = chatRepository;
    }

    public async Task<List<ChatMessageResponse>> Handle(
        GetMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId);

        if (task is null)
            throw new NotFoundException("Task not found.");

        await EnsureParticipant(task.Id, task.ShopkeeperId, request.RequesterId, request.Role);

        var messages = await _chatRepository.GetMessagesByTaskIdAsync(task.Id);

        return messages.Select(m => new ChatMessageResponse
        {
            MessageId = m.Id,
            SenderId = m.SenderId,
            SenderName = m.Sender.FullName,
            Message = m.Message,
            SentAt = m.SentAt
        }).ToList();
    }

    private async Task EnsureParticipant(
        int taskId,
        int shopkeeperId,
        int requesterId,
        string role)
    {
        if (role == "Admin")
            return;

        if (role == "Manager" && shopkeeperId == requesterId)
            return;

        var acceptedApplication =
            await _taskRepository.GetAcceptedApplicationAsync(taskId, requesterId);

        if (acceptedApplication is not null)
            return;

        throw new ForbiddenException(
            "Only the task's manager and its accepted worker can use this chat.");
    }
}