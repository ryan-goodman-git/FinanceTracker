using FinanceTracker.Application.Commands.EndRecurringTransaction;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Tests.Commands.EndRecurringTransaction;

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
    public void Handle_ShouldEndRecurringTransaction_WhenUserExists()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var user = CreateValidUser();
        var handler = new Handler(repository);

        var recurringTransaction = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            "Rent",
            1000m,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            new DateOnly(2026, 1, 10),
            null,
            10);
        
        user.AddRecurringTransaction(recurringTransaction);
        
        repository.Add(user);

        var command = new Command(
            user.Id,
            recurringTransaction.Id,
            new DateOnly(2026, 1, 30));
        
        // Act
        handler.Handle(command);

        //Assert
        var updatedRecurringTransaction = user.RecurringTransactions.Single(x => x.Id == recurringTransaction.Id);
        
        Assert.Equal(new DateOnly(2026, 1, 30), updatedRecurringTransaction.EndDate);
        Assert.Equal(recurringTransaction.StartDate, updatedRecurringTransaction.StartDate);
        Assert.Equal(recurringTransaction.Id, updatedRecurringTransaction.Id);
        Assert.Equal(recurringTransaction.UserId, updatedRecurringTransaction.UserId);
        Assert.Equal(recurringTransaction.Description, updatedRecurringTransaction.Description);
        Assert.Equal(recurringTransaction.Amount, updatedRecurringTransaction.Amount);
        Assert.Equal(recurringTransaction.Type, updatedRecurringTransaction.Type);
        Assert.Equal(recurringTransaction.ScheduledDayOfMonth, updatedRecurringTransaction.ScheduledDayOfMonth);
    }
}