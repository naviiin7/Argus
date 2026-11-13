using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Interfaces;

public interface IChatRepository
{
    Task AddMessageAsync(ChatMessage message);

    Task<List<ChatMessage>> GetMessagesByTaskIdAsync(int taskId);

    /// <summary>Latest message (with sender) for each of the given tasks that has any.</summary>
    Task<List<ChatMessage>> GetLastMessagesByTaskIdsAsync(IReadOnlyCollection<int> taskIds);

    Task SaveChangesAsync();
}