using Microsoft.EntityFrameworkCore;
using ShiftLess.Application.Interfaces;
using ShiftLess.Domain.Entities;
using ShiftLess.Persistence.Context;

namespace ShiftLess.Persistence.Repositories;

public class OtpRepository : IOtpRepository
{
    private readonly ApplicationDbContext _context;

    public OtpRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OtpCode otp)
    {
        await _context.OtpCodes.AddAsync(otp);
    }

    public async Task<OtpCode?> GetLatestAsync(string target, string purpose)
    {
        return await _context.OtpCodes
            .Where(x => x.Target == target && x.Purpose == purpose)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<OtpCode?> GetLatestActiveAsync(string target, string purpose)
    {
        var now = DateTime.UtcNow;

        return await _context.OtpCodes
            .Where(x =>
                x.Target == target &&
                x.Purpose == purpose &&
                x.ConsumedAt == null &&
                x.ExpiresAt > now)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
    }

    // Marks active codes consumed in the change tracker only; the caller
    // decides when to SaveChanges so this can be part of an atomic send.
    public async Task InvalidateActiveAsync(string target, string purpose)
    {
        var now = DateTime.UtcNow;

        var active = await _context.OtpCodes
            .Where(x =>
                x.Target == target &&
                x.Purpose == purpose &&
                x.ConsumedAt == null &&
                x.ExpiresAt > now)
            .ToListAsync();

        foreach (var otp in active)
        {
            otp.ConsumedAt = now;
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}