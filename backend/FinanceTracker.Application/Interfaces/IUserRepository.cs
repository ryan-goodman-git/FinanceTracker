using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Interfaces;

public interface IUserRepository
{
    IReadOnlyCollection<User> GetAll();
    User? GetById(Guid userId);
    void Add(User user);
    void Update(User user);
}
