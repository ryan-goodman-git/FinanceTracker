using FinanceTracker.Application.Queries.GetUserById;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Tests.Queries.GetUserById;

public class HandlerTests
{
    private static User CreateValidUser(string name = "John", Guid? id = null)
    {
        return User.Create(
            id ?? Guid.NewGuid(),
            name,
            1000m,
            new DateOnly(2026, 1, 1),
            2000m,
            25);
    }

    [Fact]
    public void Handle_ShouldReturnUser_WhenUserExists()
    {
        var repository = new FakeUserRepository();
        var user = CreateValidUser();

        repository.Add(user);

        var handler = new Handler(repository);
        var query = new Query(user.Id);

        var result = handler.Handle(query);

        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.InitialBalance, result.InitialBalance);
        Assert.Equal(user.StartDate, result.StartDate);
    }

    [Fact]
    public void Handle_ShouldThrow_WhenUserDoesNotExist()
    {
        var repository = new FakeUserRepository();
        var handler = new Handler(repository);

        var exception = Assert.Throws<InvalidOperationException>(() => handler.Handle(new Query(Guid.NewGuid())));

        Assert.Equal("User was not found.", exception.Message);
    }
}