using Microsoft.EntityFrameworkCore;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;
using ShiftLess.Persistence.Context;

namespace ShiftLess.Persistence.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly ApplicationDbContext _context;

    public ChatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddMessageAsync(ChatMessage message)
    {
        await _context.ChatMessages.AddAsync(message);
    }

    public async Task<List<ChatMessage>> GetMessagesByTaskIdAsync(int taskId)
    {
        return await _context.ChatMessages
            .Include(x => x.Sender)
            .Where(x => x.TaskRequestId == taskId)
            .OrderBy(x => x.SentAt)
            .ToListAsync();
    }

    public async Task<List<ChatMessage>> GetLastMessagesByTaskIdsAsync(
        IReadOnlyCollection<int> taskIds)
    {
        if (taskIds.Count == 0)
            return new List<ChatMessage>();

        var ids = taskIds.ToList();

        // Ids are identity values, so the highest id per task is its latest message.
        var latestIds = await _context.ChatMessages
            .Where(m => ids.Contains(m.TaskRequestId))
            .GroupBy(m => m.TaskRequestId)
            .Select(g => g.Max(m => m.Id))
            .ToListAsync();

        return await _context.ChatMessages
            .Include(m => m.Sender)
            .Where(m => latestIds.Contains(m.Id))
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}