using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);

    Task<RefreshToken?> GetByTokenAsync(string token);

    Task SaveChangesAsync();
}