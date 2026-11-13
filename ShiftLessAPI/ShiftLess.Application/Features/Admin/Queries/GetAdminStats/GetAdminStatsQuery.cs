using MediatR;

namespace ShiftLess.Application.Features.Admin.Queries.GetAdminStats;

public record GetAdminStatsQuery() : IRequest<AdminStatsResponse>;