using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Tests.Entities;

public class OneOffTransactionTests
{
    [Fact]
    public void Constructor_ShouldCreateOneOffTransaction_WhenValuesAreValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var date = new DateOnly(2026, 1, 10);

        // Act
        var transaction = new OneOffTransaction(
            id,
            userId,
            "Bonus",
            500m,
            TransactionType.Income,
            date);

        // Assert
        Assert.Equal(id, transaction.Id);
        Assert.Equal(userId, transaction.UserId);
        Assert.Equal("Bonus", transaction.Description);
        Assert.Equal(500m, transaction.Amount);
        Assert.Equal(TransactionType.Income, transaction.Type);
        Assert.Equal(date, transaction.Date);
    }

    [Fact]
    public void Constructor_ShouldTrimDescription()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var transaction = new OneOffTransaction(
            id,
            userId,
            "  Bonus  ",
            500m,
            TransactionType.Income,
            new DateOnly(2026, 1, 10));

        // Assert
        Assert.Equal("Bonus", transaction.Description);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenIdIsEmpty()
    {
        // Arrange
        var id = Guid.Empty;
        var userId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            new OneOffTransaction(
                id,
                userId,
                "Bonus",
                500m,
                TransactionType.Income,
                new DateOnly(2026, 1, 10)));

        // Assert
        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenUserIdIsEmpty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.Empty;

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            new OneOffTransaction(
                id,
                userId,
                "Bonus",
                500m,
                TransactionType.Income,
                new DateOnly(2026, 1, 10)));

        // Assert
        Assert.Equal("userId", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenDescriptionIsBlank()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            new OneOffTransaction(
                id,
                userId,
                "",
                500m,
                TransactionType.Income,
                new DateOnly(2026, 1, 10)));

        // Assert
        Assert.Equal("description", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenDescriptionIsWhitespace()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            new OneOffTransaction(
                id,
                userId,
                "   ",
                500m,
                TransactionType.Income,
                new DateOnly(2026, 1, 10)));

        // Assert
        Assert.Equal("description", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenAmountIsZero()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new OneOffTransaction(
                id,
                userId,
                "Bonus",
                0m,
                TransactionType.Income,
                new DateOnly(2026, 1, 10)));

        // Assert
        Assert.Equal("amount", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenAmountIsNegative()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new OneOffTransaction(
                id,
                userId,
                "Bonus",
                -1m,
                TransactionType.Income,
                new DateOnly(2026, 1, 10)));

        // Assert
        Assert.Equal("amount", exception.ParamName);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateDescriptionAndAmount()
    {
        // Arrange
        var transaction = CreateValidOneOffTransaction();

        // Act
        transaction.UpdateDetails("Updated bonus", 750m);

        // Assert
        Assert.Equal("Updated bonus", transaction.Description);
        Assert.Equal(750m, transaction.Amount);
    }

    [Fact]
    public void UpdateDetails_ShouldTrimDescription()
    {
        // Arrange
        var transaction = CreateValidOneOffTransaction();

        // Act
        transaction.UpdateDetails("  Updated bonus  ", 750m);

        // Assert
        Assert.Equal("Updated bonus", transaction.Description);
        Assert.Equal(750m, transaction.Amount);
    }

    [Fact]
    public void UpdateDetails_ShouldThrow_WhenDescriptionIsBlank()
    {
        // Arrange
        var transaction = CreateValidOneOffTransaction();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            transaction.UpdateDetails("", 750m));

        // Assert
        Assert.Equal("description", exception.ParamName);
        Assert.Equal("Bonus", transaction.Description);
        Assert.Equal(500m, transaction.Amount);
    }

    [Fact]
    public void UpdateDetails_ShouldThrow_WhenDescriptionIsWhitespace()
    {
        // Arrange
        var transaction = CreateValidOneOffTransaction();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            transaction.UpdateDetails("   ", 750m));

        // Assert
        Assert.Equal("description", exception.ParamName);
        Assert.Equal("Bonus", transaction.Description);
        Assert.Equal(500m, transaction.Amount);
    }

    [Fact]
    public void UpdateDetails_ShouldThrow_WhenAmountIsZero()
    {
        // Arrange
        var transaction = CreateValidOneOffTransaction();

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            transaction.UpdateDetails("Updated bonus", 0m));

        // Assert
        Assert.Equal("amount", exception.ParamName);
        Assert.Equal("Bonus", transaction.Description);
        Assert.Equal(500m, transaction.Amount);
    }

    [Fact]
    public void UpdateDetails_ShouldThrow_WhenAmountIsNegative()
    {
        // Arrange
        var transaction = CreateValidOneOffTransaction();

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            transaction.UpdateDetails("Updated bonus", -1m));

        // Assert
        Assert.Equal("amount", exception.ParamName);
        Assert.Equal("Bonus", transaction.Description);
        Assert.Equal(500m, transaction.Amount);
    }

    private static OneOffTransaction CreateValidOneOffTransaction()
    {
        return new OneOffTransaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Bonus",
            500m,
            TransactionType.Income,
            new DateOnly(2026, 1, 10));
    }
}
