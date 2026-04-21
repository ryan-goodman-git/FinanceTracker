using FinanceTracker.Application.Commands.AddOneOffTransaction;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Tests.Commands.AddOneOffTransaction;

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
    public void Handle_ShouldAddOneOffTransactionAndReturnId_WhenUserExists()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var user = CreateValidUser();
        
        repository.Add(user);
        
        var handler = new Handler(repository);

        var command = new Command(
            user.Id,
            "Food",
            50m,
            TransactionType.Expense,
            new DateOnly(2026, 1, 10));
        
        // Act
        var result = handler.Handle(command);

        // Assert
        Assert.NotEqual(Guid.Empty, result.OneOffTransactionId);

        var transaction = user.OneOffTransactions.Single();
        Assert.NotNull(transaction);
        Assert.Equal(user.Id, transaction.UserId);
        Assert.Equal("Food", transaction.Description);
        Assert.Equal(50m, transaction.Amount);
        Assert.Equal(TransactionType.Expense, transaction.Type);
        Assert.Equal(new DateOnly(2026, 1, 10), transaction.Date);
    }
}