using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);

    string GenerateRefreshToken();

    DateTime GetRefreshTokenExpiry();
}