using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Chat.DTOs;

public class SendChatMessageRequest
{
    public string Message { get; set; } = string.Empty;
}