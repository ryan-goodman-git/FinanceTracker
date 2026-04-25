using Microsoft.EntityFrameworkCore;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Infrastructure.Persistence;

public class FinanceTrackerDbContext : DbContext
{
    public FinanceTrackerDbContext(DbContextOptions<FinanceTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceTrackerDbContext).Assembly);
    }
}