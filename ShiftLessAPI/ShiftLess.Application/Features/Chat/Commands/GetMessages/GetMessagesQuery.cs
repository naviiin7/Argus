using MediatR;
using ShiftLess.Application.Features.Chat.Responses;

namespace ShiftLess.Application.Features.Chat.Queries.GetMessages;

public record GetMessagesQuery(
    int TaskId,
    int RequesterId,
    string Role)
    : IRequest<List<ChatMessageResponse>>;