using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Interfaces;

public interface IOtpRepository
{
    Task AddAsync(OtpCode otp);

    Task<OtpCode?> GetLatestAsync(string target, string purpose);

    Task<OtpCode?> GetLatestActiveAsync(string target, string purpose);

    Task InvalidateActiveAsync(string target, string purpose);

    Task SaveChangesAsync();
}