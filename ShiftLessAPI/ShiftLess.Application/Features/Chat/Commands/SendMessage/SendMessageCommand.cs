using MediatR;
using ShiftLess.Application.Features.Chat.DTOs;
using ShiftLess.Application.Features.Chat.Responses;

namespace ShiftLess.Application.Features.Chat.Commands.SendMessage;

public record SendMessageCommand(
    int TaskId,
    int SenderId,
    string Role,
    SendChatMessageRequest Request)
    : IRequest<ChatMessageResponse>;