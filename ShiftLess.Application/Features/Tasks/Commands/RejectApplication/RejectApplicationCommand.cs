using MediatR;

namespace ShiftLess.Application.Features.Tasks.Commands.RejectApplication;

public record RejectApplicationCommand(
    int TaskId,
    int ApplicationId,
    int ShopkeeperId)
    : IRequest<RejectApplicationResponse>;