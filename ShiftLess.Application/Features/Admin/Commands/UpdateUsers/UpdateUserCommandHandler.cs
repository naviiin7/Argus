using MediatR;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Features.Admin.Commands.UpdateUsers;
using ShiftLess.Application.Features.Admin.Responses;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Enums;

namespace ShiftLess.Application.Features.Admin.Commands.UpdateUser;

public class UpdateUserCommandHandler
    : IRequestHandler<UpdateUserCommand, AdminActionResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AdminActionResponse> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user is null)
            throw new NotFoundException("User not found.");

        if (user.Role == UserRole.Admin)
            throw new ForbiddenException(
                "Admin accounts can't be edited from this page.");

        if (!Enum.TryParse<UserRole>(request.Request.Role, true, out var newRole) ||
            (newRole != UserRole.Manager && newRole != UserRole.Client))
        {
            throw new BadRequestException("Role must be Manager or Worker.");
        }

        var roleChanged = user.Role != newRole;

        user.FullName = request.Request.FullName.Trim();
        user.Phone = request.Request.Phone.Trim();
        user.Role = newRole;

        await _userRepository.SaveChangesAsync();

        // The role is baked into the login token, so a role change signs
        // the user out of long-lived sessions and they pick up the new
        // role at their next login.
        if (roleChanged)
        {
            await _refreshTokenRepository.RevokeAllForUserAsync(user.Id);
        }

        return new AdminActionResponse
        {
            Message = roleChanged
                ? $"{user.FullName} updated. Their role change applies at their next login."
                : $"{user.FullName} updated."
        };
    }
}