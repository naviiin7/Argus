using MediatR;

namespace ShiftLess.Application.Features.Tasks.Commands.AcceptApplication;

public record AcceptApplicationCommand(
    int TaskId,
    int ApplicationId,
    int ShopkeeperId,
    string Role)
    : IRequest<AcceptApplicationResponse>;