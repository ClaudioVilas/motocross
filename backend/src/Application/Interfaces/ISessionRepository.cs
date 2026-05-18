using Motocross.Domain.Entities;

namespace Motocross.Application.Interfaces;

/// <summary>
/// Repository interface for Session aggregate
/// </summary>
public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(Guid id);
    Task<Session?> GetActiveSessionAsync();
    Task<List<Session>> GetAllAsync(int skip = 0, int take = 20);
    Task<Session> AddAsync(Session session);
    Task UpdateAsync(Session session);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> CountAsync();
}
