using MediatR;
using ShiftLess.Application.Features.Chat.Responses;

namespace ShiftLess.Application.Features.Chat.Queries.GetConversations;

public record GetConversationsQuery(int ManagerId)
    : IRequest<List<ConversationResponse>>;