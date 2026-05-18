using Microsoft.EntityFrameworkCore;
using Motocross.Application.Interfaces;
using Motocross.Domain.Entities;
using Motocross.Infrastructure.Persistence;

namespace Motocross.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MotocrossDbContext _dbContext;

    public UserRepository(MotocrossDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public Task AddAsync(UserAccount user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Add(user);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
