using FinanceTracker.Application.Queries.GetBalanceForUserOnDate;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Tests.Queries.GetBalanceForUserOnDate;

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
    public void Handle_ShouldReturnBalance_WhenUserExists()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var user = CreateValidUser();
        
        repository.Add(user);
         
        var handler = new Handler(repository);
        
        var query = new Query(user.Id, new DateOnly(2026, 1, 2)); 
        
        //Act
        var result = handler.Handle(query);
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(1000, result.Balance);
    }
    
    [Fact]
    public void Handle_Should_ThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var handler = new Handler(repository);
        var query = new Query(Guid.NewGuid(), new DateOnly(2026, 1, 2));
        
        //Act
        var exception = Assert.Throws<InvalidOperationException>(() => handler.Handle(query));
        
        //Assert
        Assert.NotNull(exception);
        Assert.Equal("User was not found.", exception.Message);
    }
}
