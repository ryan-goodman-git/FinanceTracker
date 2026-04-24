using FinanceTracker.Application.Commands.CreateUser;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Tests.Commands.CreateUser;

public class HandlerTests
{
    [Fact]
    public void Handle_ShouldCreateUserAndReturnId_WhenCommandIsValid()
    {
        // Arrange
        var repository = new FakeUserRepository();
        
        var handler = new Handler(repository);
        
        var command = new Command(
            "Ryan",
            1000m,
            new DateOnly(2026,1,1),
            2500m,
            1);
        
        // Act
        var result = handler.Handle(command);
        
        //Assert
        var newUser = repository.GetById(result.UserId);
        
        Assert.NotNull(newUser);
        Assert.NotEqual(Guid.Empty, result.UserId);
        
        Assert.Equal("Ryan", newUser.Name);
        Assert.Equal(1000m, newUser.InitialBalance);
        Assert.Equal(new DateOnly(2026,1,1), newUser.StartDate);

        Assert.Single(newUser.RecurringTransactions);
        var salary = newUser.RecurringTransactions.Single();
        
        Assert.Equal(2500m, salary.Amount);
        Assert.Equal(new DateOnly(2026,1,1), salary.StartDate);
        Assert.Equal(1, salary.ScheduledDayOfMonth);
        Assert.Equal("Salary", salary.Description);
        Assert.Equal(TransactionType.Income, salary.Type);
        Assert.Equal(RecurringTransactionKind.Salary, salary.Kind);
        Assert.Null(salary.EndDate);
    }
}