using MediatR;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Features.Admin.Responses;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Enums;

namespace ShiftLess.Application.Features.Admin.Commands.SetUserActive;

public class SetUserActiveCommandHandler
    : IRequestHandler<SetUserActiveCommand, AdminActionResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public SetUserActiveCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AdminActionResponse> Handle(
        SetUserActiveCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserId == request.AdminId)
            throw new BadRequestException("You can't block your own account.");

        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user is null)
            throw new NotFoundException("User not found.");

        if (user.Role == UserRole.Admin)
            throw new ForbiddenException("Admin accounts can't be blocked.");

        user.IsActive = request.IsActive;

        await _userRepository.SaveChangesAsync();

        if (!request.IsActive)
        {
            // Sign them out of every long-lived session.
            await _refreshTokenRepository.RevokeAllForUserAsync(user.Id);
        }

        return new AdminActionResponse
        {
            Message = request.IsActive
                ? $"{user.FullName} can log in again."
                : $"{user.FullName} has been blocked and signed out."
        };
    }
}