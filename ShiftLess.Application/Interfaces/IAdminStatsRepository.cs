using ShiftLess.Application.Features.Admin.Queries.GetAdminStats;

namespace ShiftLess.Application.Interfaces;

public interface IAdminStatsRepository
{
    Task<AdminStatsResponse> GetStatsAsync();
}