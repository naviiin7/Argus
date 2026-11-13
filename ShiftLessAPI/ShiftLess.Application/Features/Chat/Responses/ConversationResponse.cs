using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Chat.Responses;

public class ConversationResponse
{
    public int TaskId { get; set; }

    public string TaskTitle { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int AcceptedWorkers { get; set; }

    public string? LastMessageSender { get; set; }

    public string? LastMessageText { get; set; }

    public DateTime? LastMessageAt { get; set; }
}