namespace FinanceTracker.Application.Tests.Queries.GetProjectedSavingsForUser;

using FinanceTracker.Application.Queries.GetProjectedSavingsForUser;
using FinanceTracker.Application.Tests.Fakes;
using FinanceTracker.Domain.Entities;

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
    public void Handle_ShouldReturnProjectedSavingsAndCycleEndDate_WhenUserExists()
    {
        // Arrange
        var repository = new FakeUserRepository();

        var user = CreateValidUser();
        repository.Add(user);

        var handler = new Handler(repository);

        var query = new Query(user.Id, new DateOnly(2026, 1, 26));

        // Act
        var result = handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.GetProjectedSavingsForCurrentCycle(query.Today), result.ProjectedSavings);
        Assert.Equal(user.GetCurrentCycleEndDate(query.Today), result.CycleEndDate);
    }
    
    [Fact]
    public void Handle_Should_ThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var handler = new Handler(repository);
        var query = new Query(Guid.NewGuid(), new DateOnly(2026, 1, 26));
        
        // Act 
        var exception = Assert.Throws<InvalidOperationException>(() => handler.Handle(query));
        
        // Assert
        Assert.Equal("User was not found.", exception.Message);
    }
}