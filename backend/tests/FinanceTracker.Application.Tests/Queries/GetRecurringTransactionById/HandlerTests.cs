using FinanceTracker.Application.Queries.GetRecurringTransactionById;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Tests.Queries.GetRecurringTransactionById;

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
        var transaction = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            "Rent",
            800m,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            new DateOnly(2026, 1, 5),
            null,
            5);

        user.AddRecurringTransaction(transaction);
        repository.Add(user);

        var handler = new Handler(repository);
        var query = new Query(user.Id, transaction.Id);

        var result = handler.Handle(query);

        Assert.Equal(transaction.Id, result.RecurringTransactionId);
        Assert.Equal(transaction.Description, result.Description);
        Assert.Equal(transaction.Amount, result.Amount);
        Assert.Equal(transaction.Type, result.Type);
        Assert.Equal(transaction.Kind, result.Kind);
        Assert.Equal(transaction.ScheduledDayOfMonth, result.ScheduledDayOfMonth);
        Assert.Equal(transaction.StartDate, result.StartDate);
        Assert.Equal(transaction.EndDate, result.EndDate);
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

        Assert.Equal("Recurring transaction was not found.", exception.Message);
    }
}