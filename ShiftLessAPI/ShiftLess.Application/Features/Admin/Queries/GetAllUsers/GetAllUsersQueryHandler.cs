using MediatR;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Application.Features.Admin.Queries.GetAllUsers;

public class GetAllUsersQueryHandler
    : IRequestHandler<GetAllUsersQuery, List<GetAllUsersResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<GetAllUsersResponse>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllUsersAsync();

        var withActivity = await _userRepository.GetUserIdsWithActivityAsync();

        return users
            .OrderBy(u => u.Role)
            .ThenBy(u => u.FullName)
            .Select(u => new GetAllUsersResponse
            {
                UserId = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                HasActivity = withActivity.Contains(u.Id)
            })
            .ToList();
    }
}