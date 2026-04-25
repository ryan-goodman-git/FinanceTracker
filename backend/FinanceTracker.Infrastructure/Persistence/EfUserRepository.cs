using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Infrastructure.Persistence;

public class EfUserRepository : IUserRepository
{
    private readonly FinanceTrackerDbContext _dbContext;
    
    public EfUserRepository(FinanceTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public User? GetById(Guid userId)
    {
        throw new NotImplementedException();
    }

    public void Add(User user)
    {
        throw new NotImplementedException();
    }

    public void Update(User user)
    {
        throw new NotImplementedException();
    }
}