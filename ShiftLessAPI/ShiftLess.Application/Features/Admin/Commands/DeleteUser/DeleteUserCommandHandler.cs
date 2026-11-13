using MediatR;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Features.Admin.Responses;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Enums;

namespace ShiftLess.Application.Features.Admin.Commands.DeleteUser;

public class DeleteUserCommandHandler
    : IRequestHandler<DeleteUserCommand, AdminActionResponse>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AdminActionResponse> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserId == request.AdminId)
            throw new BadRequestException("You can't delete your own account.");

        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user is null)
            throw new NotFoundException("User not found.");

        if (user.Role == UserRole.Admin)
            throw new ForbiddenException("Admin accounts can't be deleted.");

        if (await _userRepository.HasActivityAsync(user.Id))
        {
            throw new ConflictException(
                "This user has tasks, applications or chat messages, so they can't be deleted. " +
                "Block the account instead.");
        }

        var name = user.FullName;

        await _userRepository.DeleteAsync(user);

        return new AdminActionResponse
        {
            Message = $"{name} was deleted."
        };
    }
}