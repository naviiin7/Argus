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

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}