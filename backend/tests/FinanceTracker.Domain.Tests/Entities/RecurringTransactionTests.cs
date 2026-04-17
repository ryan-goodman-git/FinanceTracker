using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Tests.Entities;

public class RecurringTransactionTests
{
    [Fact]
    public void Constructor_ShouldCreateRecurringTransaction_WhenValuesAreValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var startDate = new DateOnly(2026, 1, 1);
        var endDate = new DateOnly(2026, 12, 31);

        // Act
        var transaction = new RecurringTransaction(
            id,
            userId,
            "Gym",
            50m,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            startDate,
            endDate,
            15);

        // Assert
        Assert.Equal(id, transaction.Id);
        Assert.Equal(userId, transaction.UserId);
        Assert.Equal("Gym", transaction.Description);
        Assert.Equal(50m, transaction.Amount);
        Assert.Equal(TransactionType.Expense, transaction.Type);
        Assert.Equal(RecurringTransactionKind.Expense, transaction.Kind);
        Assert.Equal(startDate, transaction.StartDate);
        Assert.Equal(endDate, transaction.EndDate);
        Assert.Equal(15, transaction.ScheduledDayOfMonth);
    }

    [Fact]
    public void Constructor_ShouldTrimDescription()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var transaction = new RecurringTransaction(
            id,
            userId,
            "  Gym  ",
            50m,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            new DateOnly(2026, 1, 1),
            null,
            15);

        // Assert
        Assert.Equal("Gym", transaction.Description);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenIdIsEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            new RecurringTransaction(
                Guid.Empty,
                userId,
                "Gym",
                50m,
                TransactionType.Expense,
                RecurringTransactionKind.Expense,
                new DateOnly(2026, 1, 1),
                null,
                15));

        // Assert
        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenUserIdIsEmpty()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            new RecurringTransaction(
                id,
                Guid.Empty,
                "Gym",
                50m,
                TransactionType.Expense,
                RecurringTransactionKind.Expense,
                new DateOnly(2026, 1, 1),
                null,
                15));

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
            new RecurringTransaction(
                id,
                userId,
                "",
                50m,
                TransactionType.Expense,
                RecurringTransactionKind.Expense,
                new DateOnly(2026, 1, 1),
                null,
                15));

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
            new RecurringTransaction(
                id,
                userId,
                "   ",
                50m,
                TransactionType.Expense,
                RecurringTransactionKind.Expense,
                new DateOnly(2026, 1, 1),
                null,
                15));

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
            new RecurringTransaction(
                id,
                userId,
                "Gym",
                0m,
                TransactionType.Expense,
                RecurringTransactionKind.Expense,
                new DateOnly(2026, 1, 1),
                null,
                15));

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
            new RecurringTransaction(
                id,
                userId,
                "Gym",
                -1m,
                TransactionType.Expense,
                RecurringTransactionKind.Expense,
                new DateOnly(2026, 1, 1),
                null,
                15));

        // Assert
        Assert.Equal("amount", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenScheduledDayOfMonthIsZero()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new RecurringTransaction(
                id,
                userId,
                "Gym",
                50m,
                TransactionType.Expense,
                RecurringTransactionKind.Expense,
                new DateOnly(2026, 1, 1),
                null,
                0));

        // Assert
        Assert.Equal("scheduledDayOfMonth", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenScheduledDayOfMonthIsGreaterThan31()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new RecurringTransaction(
                id,
                userId,
                "Gym",
                50m,
                TransactionType.Expense,
                RecurringTransactionKind.Expense,
                new DateOnly(2026, 1, 1),
                null,
                32));

        // Assert
        Assert.Equal("scheduledDayOfMonth", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            new RecurringTransaction(
                id,
                userId,
                "Gym",
                50m,
                TransactionType.Expense,
                RecurringTransactionKind.Expense,
                new DateOnly(2026, 2, 1),
                new DateOnly(2026, 1, 31),
                15));

        // Assert
        Assert.Equal("endDate", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenSalaryIsNotIncome()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            new RecurringTransaction(
                id,
                userId,
                "Salary",
                2000m,
                TransactionType.Expense,
                RecurringTransactionKind.Salary,
                new DateOnly(2026, 1, 1),
                null,
                25));

        // Assert
        Assert.Equal("Salary must be income.", exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenExpenseKindIsNotExpenseType()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            new RecurringTransaction(
                id,
                userId,
                "Gym",
                50m,
                TransactionType.Income,
                RecurringTransactionKind.Expense,
                new DateOnly(2026, 1, 1),
                null,
                15));

        // Assert
        Assert.Equal("Expense must be an expense.", exception.Message);
    }

    [Fact]
    public void EndOn_ShouldSetEndDate()
    {
        // Arrange
        var transaction = CreateValidRecurringExpense();

        // Act
        transaction.EndOn(new DateOnly(2026, 2, 1));

        // Assert
        Assert.Equal(new DateOnly(2026, 2, 1), transaction.EndDate);
    }

    [Fact]
    public void EndOn_ShouldAllowEndDateEqualToStartDate()
    {
        // Arrange
        var startDate = new DateOnly(2026, 1, 1);
        var transaction = CreateValidRecurringExpense(startDate: startDate);

        // Act
        transaction.EndOn(startDate);

        // Assert
        Assert.Equal(startDate, transaction.EndDate);
    }

    [Fact]
    public void EndOn_ShouldThrow_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        var transaction = CreateValidRecurringExpense(startDate: new DateOnly(2026, 2, 1));

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
            transaction.EndOn(new DateOnly(2026, 1, 31)));

        // Assert
        Assert.Equal("Recurring transaction cannot end before it starts.", exception.Message);
        Assert.Null(transaction.EndDate);
    }

    [Fact]
    public void EndOn_ShouldThrow_WhenTransactionHasAlreadyEnded()
    {
        // Arrange
        var transaction = CreateValidRecurringExpense();

        transaction.EndOn(new DateOnly(2026, 2, 1));

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
            transaction.EndOn(new DateOnly(2026, 2, 2)));

        // Assert
        Assert.Equal("Recurring transaction has already ended.", exception.Message);
        Assert.Equal(new DateOnly(2026, 2, 1), transaction.EndDate);
    }

    private static RecurringTransaction CreateValidRecurringExpense(
        DateOnly? startDate = null,
        DateOnly? endDate = null)
    {
        return new RecurringTransaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Gym",
            50m,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            startDate ?? new DateOnly(2026, 1, 1),
            endDate,
            15);
    }
}
