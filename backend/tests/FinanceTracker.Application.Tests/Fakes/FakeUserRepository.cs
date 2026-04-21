namespace FinanceTracker.Application.Tests.Fakes;

using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

public class FakeUserRepository : IUserRepository
{
    private readonly Dictionary<Guid, User> _users = new();

    public User? GetById(Guid id)
    {
        _users.TryGetValue(id, out var user);
        return user;
    }

    public void Add(User user)
    {
        _users[user.Id] = user;
    }

    public void Update(User user)
    {
        _users[user.Id] = user;
    }
}