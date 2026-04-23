using FinanceTracker.Application.Commands.EditOneOffTransaction;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Tests.Commands.EditOneOffTransaction;

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
    public void Handle_ShouldEditOneOffTransaction_WhenUserExists()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var user = CreateValidUser();

        var oneOffTransaction = new OneOffTransaction(
            Guid.NewGuid(),
            user.Id,
            "Food",
            100m,
            TransactionType.Expense,
            new DateOnly(2026, 1, 2));
        
        user.AddOneOffTransaction(oneOffTransaction);
        
        repository.Add(user);
        
        var handler = new Handler(repository);

        var command = new Command(
            user.Id,
            oneOffTransaction.Id,
            "Updated Food", 
            200m);
        
        // Act
        handler.Handle(command);
        
        //Assert
        var updatedTransaction = user.OneOffTransactions.Single();
        Assert.Equal(oneOffTransaction.Id, updatedTransaction.Id);
        Assert.Equal("Updated Food", updatedTransaction.Description);
        Assert.Equal(200m, updatedTransaction.Amount);
    }
    
    [Fact]
    public void Handle_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var handler = new Handler(repository);
        var command = new Command(Guid.NewGuid(), Guid.NewGuid(), "Updated Food", 200m);
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => handler.Handle(command));
        
        // Assert
        Assert.NotNull(exception);
        Assert.Equal("User was not found.", exception.Message);
        
    }
}