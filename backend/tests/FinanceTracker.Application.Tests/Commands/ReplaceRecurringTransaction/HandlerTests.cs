using FinanceTracker.Application.Commands.ReplaceRecurringTransaction;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Tests.Commands.ReplaceRecurringTransaction;

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
    public void Handle_ShouldReplaceRecurringTransactionAndReturnNewId_WhenUserExists()
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
            "New Rent",
            1500m,
            1,
            new DateOnly(2026, 2, 1));

        // Act
        var result = handler.Handle(command);
            
        //Assert
        var replacementTransaction = user.RecurringTransactions.Single(t => t.Id == result.RecurringTransactionId);
        var originalTransaction = user.RecurringTransactions.Single(t => t.Id == recurringTransaction.Id);
        
        Assert.Equal(new DateOnly(2026, 1, 31), originalTransaction.EndDate);
        Assert.Equal(result.RecurringTransactionId, replacementTransaction.Id);
        Assert.Equal(command.Description, replacementTransaction.Description);
        Assert.Equal(command.Amount, replacementTransaction.Amount);
        Assert.Equal(command.ScheduledDayOfMonth, replacementTransaction.ScheduledDayOfMonth);
        Assert.Equal(TransactionType.Expense, replacementTransaction.Type);
        Assert.Equal(RecurringTransactionKind.Expense, replacementTransaction.Kind);
        Assert.Equal(command.ReplacementStartDate, replacementTransaction.StartDate);
        Assert.Null(replacementTransaction.EndDate);
    }

    [Fact]
    public void Handle_ShouldThrow_WhenUserDoesNotExist()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var handler = new Handler(repository);

        var command = new Command(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Rent",
            1500m,
            1,
            new DateOnly(2026, 2, 1));
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => handler.Handle(command));
        
        //Assert
        Assert.Equal("User was not found.", exception.Message);
    }
    
    [Fact]
    public void Handle_ShouldNotUpdateUser_WhenReplacementIsInvalid()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var user = CreateValidUser();

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

        var handler = new Handler(repository);

        var command = new Command(
            user.Id,
            recurringTransaction.Id,
            "",
            1500m,
            1,
            new DateOnly(2026, 2, 1));

        // Act
        var exception = Assert.Throws<ArgumentException>(() => handler.Handle(command));

        // Assert
        Assert.Equal("description", exception.ParamName);
        Assert.Null(recurringTransaction.EndDate);
    }
}