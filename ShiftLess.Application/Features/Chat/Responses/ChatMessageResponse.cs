using System.Collections.Generic;
using System.Text;
using System;

namespace ShiftLess.Application.Features.Chat.Responses;

public class ChatMessageResponse
{
    public int MessageId { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}