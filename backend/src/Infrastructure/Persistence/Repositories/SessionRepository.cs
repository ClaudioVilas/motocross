using Microsoft.EntityFrameworkCore;
using Motocross.Application.Interfaces;
using Motocross.Domain.Entities;
using Motocross.Domain.Enums;

namespace Motocross.Infrastructure.Persistence.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly MotocrossDbContext _context;

    public SessionRepository(MotocrossDbContext context)
    {
        _context = context;
    }

    public async Task<Session?> GetByIdAsync(Guid id)
    {
        return await _context.Sessions
            .Include(s => s.TrackingPoints)
            .Include(s => s.Laps)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Session?> GetActiveSessionAsync()
    {
        return await _context.Sessions
            .Include(s => s.TrackingPoints)
            .Include(s => s.Laps)
            .FirstOrDefaultAsync(s => s.Status == SessionStatus.Active);
    }

    public async Task<List<Session>> GetAllAsync(int skip = 0, int take = 20)
    {
        return await _context.Sessions
            .Include(s => s.Laps)
            .OrderByDescending(s => s.StartTime)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<Session> AddAsync(Session session)
    {
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task UpdateAsync(Session session)
    {
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session != null)
        {
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Sessions.AnyAsync(s => s.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Sessions.CountAsync();
    }
}
