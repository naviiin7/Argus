using System;
using System.Collections.Generic;
using System.Text;
using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Interfaces;

public interface IChatRepository
{
    Task AddMessageAsync(ChatMessage message);

    Task<List<ChatMessage>> GetMessagesByTaskIdAsync(int taskId);

    Task SaveChangesAsync();
}