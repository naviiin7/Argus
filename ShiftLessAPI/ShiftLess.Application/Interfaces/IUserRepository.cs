using ShiftLess.Domain.Entities;

namespace ShiftLess.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(int id);

    Task<List<User>> GetAllUsersAsync();

    Task AddAsync(User user);

    /// <summary>True if the user owns tasks, applications, chat messages or assignments.</summary>
    Task<bool> HasActivityAsync(int userId);

    /// <summary>Ids of every user that has any tasks, applications, chat messages or assignments.</summary>
    Task<HashSet<int>> GetUserIdsWithActivityAsync();

    /// <summary>Removes the user together with their refresh tokens and verification codes.</summary>
    Task DeleteAsync(User user);

    Task SaveChangesAsync();
}