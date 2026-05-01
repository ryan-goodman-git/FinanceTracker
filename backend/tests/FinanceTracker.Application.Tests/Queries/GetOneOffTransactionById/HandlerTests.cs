using FinanceTracker.Application.Queries.GetOneOffTransactionById;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Tests.Queries.GetOneOffTransactionById;

public class HandlerTests
{
    private static User CreateValidUser(Guid? id = null)
    {
        return User.Create(
            id ?? Guid.NewGuid(),
            "John",
            1000m,
            new DateOnly(2026, 1, 1),
            2000m,
            25);
    }

    [Fact]
    public void Handle_ShouldReturnTransaction_WhenTransactionExists()
    {
        var repository = new FakeUserRepository();
        var user = CreateValidUser();
        var transaction = new OneOffTransaction(
            Guid.NewGuid(),
            user.Id,
            "Groceries",
            50m,
            TransactionType.Expense,
            new DateOnly(2026, 1, 10));

        user.AddOneOffTransaction(transaction);
        repository.Add(user);

        var handler = new Handler(repository);
        var query = new Query(user.Id, transaction.Id);

        var result = handler.Handle(query);

        Assert.Equal(transaction.Id, result.OneOffTransactionId);
        Assert.Equal(transaction.Description, result.Description);
        Assert.Equal(transaction.Amount, result.Amount);
        Assert.Equal(transaction.Type, result.Type);
        Assert.Equal(transaction.Date, result.Date);
    }

    [Fact]
    public void Handle_ShouldThrow_WhenUserDoesNotExist()
    {
        var repository = new FakeUserRepository();
        var handler = new Handler(repository);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            handler.Handle(new Query(Guid.NewGuid(), Guid.NewGuid())));

        Assert.Equal("User was not found.", exception.Message);
    }

    [Fact]
    public void Handle_ShouldThrow_WhenTransactionDoesNotExist()
    {
        var repository = new FakeUserRepository();
        var user = CreateValidUser();

        repository.Add(user);

        var handler = new Handler(repository);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            handler.Handle(new Query(user.Id, Guid.NewGuid())));

        Assert.Equal("One-off transaction was not found.", exception.Message);
    }
}