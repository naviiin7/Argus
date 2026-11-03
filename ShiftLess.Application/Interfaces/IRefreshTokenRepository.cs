using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);

    Task<RefreshToken?> GetByTokenAsync(string token);

    /// <summary>Signs a user out everywhere by revoking all of their active refresh tokens.</summary>
    Task RevokeAllForUserAsync(int userId);

    Task SaveChangesAsync();
}