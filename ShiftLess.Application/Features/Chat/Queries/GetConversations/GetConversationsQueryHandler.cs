using MediatR;
using ShiftLess.Application.Features.Chat.Responses;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Chat.Queries.GetConversations;

public class GetConversationsQueryHandler
    : IRequestHandler<GetConversationsQuery, List<ConversationResponse>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IChatRepository _chatRepository;

    public GetConversationsQueryHandler(
        ITaskRepository taskRepository,
        IChatRepository chatRepository)
    {
        _taskRepository = taskRepository;
        _chatRepository = chatRepository;
    }

    public async Task<List<ConversationResponse>> Handle(
        GetConversationsQuery request,
        CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetTasksByManagerAsync(request.ManagerId);

        if (tasks.Count == 0)
            return new List<ConversationResponse>();

        var lastMessages = await _chatRepository.GetLastMessagesByTaskIdsAsync(
            tasks.Select(t => t.Id).ToList());

        var lastByTask = lastMessages.ToDictionary(m => m.TaskRequestId);

        var result = new List<ConversationResponse>();

        foreach (var task in tasks)
        {
            var accepted = await _taskRepository.GetAcceptedWorkerCountAsync(task.Id);

            lastByTask.TryGetValue(task.Id, out var last);

            // A conversation exists once someone is accepted or anyone has written.
            if (accepted == 0 && last is null)
                continue;

            result.Add(new ConversationResponse
            {
                TaskId = task.Id,
                TaskTitle = task.Title,
                Status = task.Status.ToString(),
                AcceptedWorkers = accepted,
                LastMessageSender = last?.Sender?.FullName,
                LastMessageText = last?.Message,
                LastMessageAt = last?.SentAt
            });
        }

        return result
            .OrderByDescending(c => c.LastMessageAt ?? DateTime.MinValue)
            .ThenBy(c => c.TaskTitle)
            .ToList();
    }
}