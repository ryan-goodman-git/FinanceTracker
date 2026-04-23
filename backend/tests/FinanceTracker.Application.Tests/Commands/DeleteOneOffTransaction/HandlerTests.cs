using FinanceTracker.Application.Commands.DeleteOneOffTransaction;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Tests.Commands.DeleteOneOffTransaction;

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
    public void Handle_ShouldDeleteOneOffTransaction_WhenUserExists()
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

        var command = new Command(user.Id, oneOffTransaction.Id);
        
        // Act
        handler.Handle(command);

        // Assert
        Assert.Empty(user.OneOffTransactions);
        Assert.DoesNotContain(user.OneOffTransactions, t => t.Id == oneOffTransaction.Id);
    }
    
    [Fact]
    public void Handle_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var handler = new Handler(repository);
        var command = new Command(Guid.NewGuid(), Guid.NewGuid());
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => handler.Handle(command));
        
        //Assert
        Assert.NotNull(exception);
        Assert.Equal("User was not found.", exception.Message);
        
    }
}