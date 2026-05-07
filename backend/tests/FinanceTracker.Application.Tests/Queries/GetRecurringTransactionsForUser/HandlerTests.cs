using FinanceTracker.Application.Queries.GetRecurringTransactionsForUser;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Tests.Queries.GetRecurringTransactionsForUser;

public class HandlerTests
{
    private static User CreateValidUser(Guid? id = null)
    {
        return User.Create(
            id ?? Guid.NewGuid(),
            "Ryan",
            1000m,
            new DateOnly(2026, 5, 1),
            2500m,
            25);
    }

    [Fact]
    public void Handle_ShouldReturnRecurringTransactions_WhenUserExists()
    {
        var repository = new FakeUserRepository();
        var user = CreateValidUser();

        var rent = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            "Rent",
            900m,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            new DateOnly(2026, 5, 1),
            null,
            1);

        user.AddRecurringTransaction(rent);
        repository.Add(user);

        var handler = new Handler(repository);

        var result = handler.Handle(new Query(user.Id));

        Assert.Equal(2, result.Count);

        Assert.Contains(result, transaction =>
            transaction.UserId == user.Id &&
            transaction.Description == "Salary" &&
            transaction.Amount == 2500m &&
            transaction.Type == TransactionType.Income &&
            transaction.Kind == RecurringTransactionKind.Salary &&
            transaction.ScheduledDayOfMonth == 25);

        Assert.Contains(result, transaction =>
            transaction.UserId == user.Id &&
            transaction.Description == "Rent" &&
            transaction.Amount == 900m &&
            transaction.Type == TransactionType.Expense &&
            transaction.Kind == RecurringTransactionKind.Expense &&
            transaction.ScheduledDayOfMonth == 1);
    }

    [Fact]
    public void Handle_ShouldThrow_WhenUserDoesNotExist()
    {
        var repository = new FakeUserRepository();
        var handler = new Handler(repository);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            handler.Handle(new Query(Guid.NewGuid())));

        Assert.Equal("User was not found.", exception.Message);
    }
}