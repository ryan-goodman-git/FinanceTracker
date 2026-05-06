using FinanceTracker.Application.Queries.GetUsers;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Tests.Queries.GetUsers;

public class HandlerTests
{
    private static User CreateValidUser(string name, Guid? id = null)
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
    public void Handle_ShouldReturnUsers_WhenUsersExist()
    {
        var repository = new FakeUserRepository();
        var firstUser = CreateValidUser("John");
        var secondUser = CreateValidUser("Jane");

        repository.Add(firstUser);
        repository.Add(secondUser);

        var handler = new Handler(repository);

        var result = handler.Handle(new Query());

        Assert.Collection(
            result.OrderBy(user => user.Name),
            user =>
            {
                Assert.Equal(secondUser.Id, user.UserId);
                Assert.Equal(secondUser.Name, user.Name);
                Assert.Equal(secondUser.InitialBalance, user.InitialBalance);
                Assert.Equal(secondUser.StartDate, user.StartDate);
            },
            user =>
            {
                Assert.Equal(firstUser.Id, user.UserId);
                Assert.Equal(firstUser.Name, user.Name);
                Assert.Equal(firstUser.InitialBalance, user.InitialBalance);
                Assert.Equal(firstUser.StartDate, user.StartDate);
            });
    }

    [Fact]
    public void Handle_ShouldReturnEmptyCollection_WhenNoUsersExist()
    {
        var repository = new FakeUserRepository();
        var handler = new Handler(repository);

        var result = handler.Handle(new Query());

        Assert.Empty(result);
    }
}
