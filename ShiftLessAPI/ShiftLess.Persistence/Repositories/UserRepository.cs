using Microsoft.EntityFrameworkCore;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;
using ShiftLess.Persistence.Context;

namespace ShiftLess.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        // PostgreSQL compares text case-sensitively (SQL Server did not),
        // so compare lower-cased on both sides.
        var normalized = (email ?? string.Empty).Trim().ToLowerInvariant();

        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == normalized);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<bool> HasActivityAsync(int userId)
    {
        return await _context.TaskRequests.AnyAsync(t => t.ShopkeeperId == userId)
            || await _context.TaskApplications.AnyAsync(a => a.WorkerId == userId)
            || await _context.ChatMessages.AnyAsync(m => m.SenderId == userId)
            || await _context.Assignments.AnyAsync(a => a.ClientId == userId);
    }

    public async Task<HashSet<int>> GetUserIdsWithActivityAsync()
    {
        var ids = new HashSet<int>();

        ids.UnionWith(await _context.TaskRequests
            .Select(t => t.ShopkeeperId).Distinct().ToListAsync());

        ids.UnionWith(await _context.TaskApplications
            .Select(a => a.WorkerId).Distinct().ToListAsync());

        ids.UnionWith(await _context.ChatMessages
            .Select(m => m.SenderId).Distinct().ToListAsync());

        ids.UnionWith(await _context.Assignments
            .Select(a => a.ClientId).Distinct().ToListAsync());

        return ids;
    }

    public async Task DeleteAsync(User user)
    {
        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == user.Id)
            .ToListAsync();

        _context.RefreshTokens.RemoveRange(tokens);

        var email = user.Email.ToLower();

        var codes = await _context.OtpCodes
            .Where(o => o.Target.ToLower() == email)
            .ToListAsync();

        _context.OtpCodes.RemoveRange(codes);

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}