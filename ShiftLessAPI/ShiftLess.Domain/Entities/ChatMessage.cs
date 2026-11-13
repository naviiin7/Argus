using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Domain.Entities;

public class ChatMessage
{
    public int Id { get; set; }

    public int TaskRequestId { get; set; }
    public TaskRequest TaskRequest { get; set; } = null!;

    public int SenderId { get; set; }
    public User Sender { get; set; } = null!;

    public string Message { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}