using FinanceTracker.Application.Commands.AddRecurringTransaction;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Tests.Commands.AddRecurringTransaction;

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
    public void Handle_ShouldAddRecurringTransactionAndReturnId_WhenUserExists()
    {
        //Arrange
        var repository = new FakeUserRepository();
        var user = CreateValidUser();
        
        repository.Add(user);
        
        var handler = new Handler(repository);

        var command = new Command(
            user.Id,
            "Rent",
            1000m,
            15,
            new DateOnly(2026, 1, 1));
        
        //Act
        var result = handler.Handle(command);
        
        //Assert
        var recurringTransaction = user.RecurringTransactions.Single(t => t.Id == result.RecurringTransactionId);
        
        Assert.Equal(user.Id, recurringTransaction.UserId);
        Assert.Equal("Rent", recurringTransaction.Description);
        Assert.Equal(1000m, recurringTransaction.Amount);
        Assert.Equal(TransactionType.Expense, recurringTransaction.Type);
        Assert.Equal(RecurringTransactionKind.Expense, recurringTransaction.Kind);
        Assert.Equal(new DateOnly(2026, 1, 1), recurringTransaction.StartDate);
        Assert.Equal(15, recurringTransaction.ScheduledDayOfMonth);
        Assert.Null(recurringTransaction.EndDate);
    }
    
    [Fact]
    public void Handle_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var handler = new Handler(repository);

        var command = new Command(
            Guid.NewGuid(),
            "Rent",
            1000m,
            15,
            new DateOnly(2026, 1, 1));

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => handler.Handle(command));
        
        // Assert
        Assert.NotNull(exception);
        Assert.Equal("User was not found.", exception.Message);
    }
}