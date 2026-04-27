using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
        return _dbContext.Users
            .Include(user => user.RecurringTransactions)
            .Include(user => user.OneOffTransactions)
            .SingleOrDefault(user => user.Id == userId);
    }

    public void Add(User user)
    {
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();
    }
}