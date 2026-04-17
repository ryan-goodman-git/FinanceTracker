using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Tests.Entities;

public class UserTests
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
    
    private static RecurringTransaction CreateValidRecurringExpense(
        Guid userId,
        string description = "Gym",
        decimal amount = 50m,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        int scheduledDayOfMonth = 15)
    {
        return new RecurringTransaction(
            Guid.NewGuid(),
            userId,
            description,
            amount,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            startDate ?? new DateOnly(2026, 1, 10),
            endDate,
            scheduledDayOfMonth);
    }
    
    [Fact]
    public void Create_ShouldCreateUserWithInitialSalary()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var startDate = new DateOnly(2026, 1, 1);
        
        //Act
        var user = User.Create(
            userId,
            "Ryan",
            1000m,
            startDate,
            2000m,
            25);
        
        //Assert
        Assert.Equal(userId, user.Id);
        Assert.Equal("Ryan", user.Name);
        Assert.Equal(1000m, user.InitialBalance);
        Assert.Equal(startDate, user.StartDate);
        
        Assert.Single(user.RecurringTransactions);

        var salary = user.RecurringTransactions.Single();
        
        Assert.Equal(userId, salary.UserId);
        Assert.Equal("Salary", salary.Description);
        Assert.Equal(2000m, salary.Amount);
        Assert.Equal(TransactionType.Income, salary.Type);
        Assert.Equal(RecurringTransactionKind.Salary, salary.Kind);
        Assert.Equal(startDate, salary.StartDate);
        Assert.Null(salary.EndDate);
        Assert.Equal(25, salary.ScheduledDayOfMonth);
    }
    
    [Fact]
    public void Create_ShouldThrow_WhenIdIsEmpty()
    {
        // Arrange
        var userId = Guid.Empty;
        var startDate = new DateOnly(2026, 1, 1);

        // Act
        Action act = () => User.Create(
            userId,
            "Ryan",
            1000m,
            startDate,
            2000m,
            25);

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrow_WhenNameIsBlank()
    {
        //Assert
        var exception = Assert.Throws<ArgumentException>(() => CreateValidUser(name: ""));
        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrow_WhenNameIsWhitespace()
    {
        //Act
        Action act = () => CreateValidUser(" ");
        
        //Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("name", exception.ParamName);
    }
    
    [Fact]
    public void GetBalanceOn_ShouldReturnInitialBalanceBeforeFirstSalaryDate()
    {
        // Arrange
        var user = CreateValidUser();
        var targetDate = new DateOnly(2026, 1, 24);

        // Act
        var balance = user.GetBalanceOn(targetDate);

        // Assert
        Assert.Equal(1000m, balance);
    }
    
    [Fact]
    public void GetBalanceOn_ShouldIncludeSalaryOnSalaryDate()
    {
        // Arrange
        var user = CreateValidUser();
        var targetDate = new DateOnly(2026, 1, 25);
        
        // Act
        var balance = user.GetBalanceOn(targetDate);
        
        // Assert
        Assert.Equal(3000m, balance);
    }

    [Fact]
    public void GetBalanceOn_ShouldIncludeSalaryAfterSalaryDate()
    {
        //Arrange
        var user = CreateValidUser();
        var targetDate = new DateOnly(2026, 1, 30);
        
        //Act
        var balance = user.GetBalanceOn(targetDate);
        
        //Assert
        Assert.Equal(3000m, balance);
    }

    [Fact]
    public void GetBalanceOn_ShouldIncludeOneOffIncome()
    {
        // Arrange
        var user = CreateValidUser();

        var transaction = new OneOffTransaction(
            Guid.NewGuid(),
            user.Id,
            "Bonus",
            500m,
            TransactionType.Income,
            new DateOnly(2026, 1, 10));

        user.AddOneOffTransaction(transaction);

        var targetDate = new DateOnly(2026, 1, 25);

        // Act
        var balance = user.GetBalanceOn(targetDate);

        // Assert
        Assert.Equal(3500m, balance);
    }

    [Fact]
    public void GetBalanceOn_ShouldIncludeOneOffExpense()
    {
        //Arrange
        var user = CreateValidUser();
        
        var transaction = new OneOffTransaction(
            Guid.NewGuid(),
            user.Id,
            "Rent",
            500m,
            TransactionType.Expense,
            new DateOnly(2026, 1, 10));
        
        user.AddOneOffTransaction(transaction);
        var targetDate = new DateOnly(2026, 1, 25);
        
        //Act
        var balance = user.GetBalanceOn(targetDate);
        
        //Assert
        Assert.Equal(2500m, balance);
    }

    [Fact]
    public void GetBalanceOn_ShouldIgnoreOneOffTransactionAfterTargetDate()
    {
        //Arrange
        var user = CreateValidUser();
        
        var transaction = new OneOffTransaction(
            Guid.NewGuid(),
            user.Id,
            "Rent",
            500m,
            TransactionType.Expense,
            new DateOnly(2026, 1, 26));
        
        user.AddOneOffTransaction(transaction);
        
        var targetDate = new DateOnly(2026, 1, 25);
        
        //Act
        var balance = user.GetBalanceOn(targetDate);
        
        //Assert
        Assert.Equal(3000m, balance);
    }

    [Fact]
    public void AddOneOffTransaction_ShouldAddTransaction()
    {
        //Arrange
        var user = CreateValidUser();
        
        var oneOffTransaction = new OneOffTransaction(
            Guid.NewGuid(),
            user.Id,
            "Bonus",
            500m,
            TransactionType.Income,
            new DateOnly(2026, 1, 10));
        
        //Act 
        user.AddOneOffTransaction(oneOffTransaction);
        
        //Assert
        var transaction = user.OneOffTransactions.Single();
        Assert.Single(user.OneOffTransactions);
        Assert.Equal(oneOffTransaction.Id, transaction.Id);
        Assert.Equal(user.Id, transaction.UserId);
        Assert.Equal(oneOffTransaction.Description, transaction.Description);
        Assert.Equal(oneOffTransaction.Amount, transaction.Amount);
        Assert.Equal(oneOffTransaction.Type, transaction.Type);
        Assert.Equal(oneOffTransaction.Date, transaction.Date);
    }

    [Fact]
    public void AddOneOffTransaction_ShouldThrow_WhenTransactionBelongsToDifferentUser()
    {
        //Arrange
        var user = CreateValidUser();
        
        var oneOffTransaction = new OneOffTransaction(
            Guid.NewGuid(),
            Guid.NewGuid(), 
            "Bonus",
            500m,
            TransactionType.Income,
            new DateOnly(2026, 1, 10));
        
        //Act
        var exception = Assert.Throws<InvalidOperationException>(() => user.AddOneOffTransaction(oneOffTransaction));
        
        //Assert
        Assert.Equal("Transaction must belong to this user.", exception.Message);
    }
    
    [Fact]
    public void AddOneOffTransaction_ShouldThrow_WhenTransactionDateIsBeforeUserStartDate()
    {
        
        //Arrange
        var user = CreateValidUser();
        
        var oneOffTransaction = new OneOffTransaction(
            Guid.NewGuid(),
            user.Id,
            "Bonus",
            500m,
            TransactionType.Income,
            new DateOnly(2025, 1, 1));
        
        //Act
        var exception = Assert.Throws<InvalidOperationException>(() => user.AddOneOffTransaction(oneOffTransaction));
        
        //Assert
        Assert.Equal("Transaction cannot be before user start date.", exception.Message);
    }

    [Fact]
    public void EditOneOffTransaction_ShouldUpdateDescriptionAndAmount()
    {
        //Arrange
        var user = CreateValidUser();
        
        var oneOffTransaction = new OneOffTransaction(
            Guid.NewGuid(),
            user.Id,
            "Bonus",
            500m,
            TransactionType.Income,
            new DateOnly(2026, 1, 10));
        
        //Act
        user.AddOneOffTransaction(oneOffTransaction);
        var transaction = user.OneOffTransactions.Single();
        
        user.EditOneOffTransaction(transaction.Id, "Work bonus", 600m);
        
        //Assert
        Assert.Equal("Work bonus", transaction.Description);
        Assert.Equal(600m, transaction.Amount);
    }
    
    [Fact]
    public void ReplaceRecurringTransaction_ShouldEndExistingTransactionAndAddReplacement()
    {
        // Arrange
        var user = CreateValidUser();
        
        var originalRecurringTransaction = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            "Gym",
            50m,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            new DateOnly(2026, 1, 10),
            null,
            15);
        
        user.AddRecurringTransaction(originalRecurringTransaction);
        
        // Act
        user.ReplaceRecurringTransaction(originalRecurringTransaction.Id, "Gym Membership", 60m, 16, new DateOnly(2026, 2, 1));
        
        // Assert
        var newRecurringTransaction = user.RecurringTransactions.Single(t =>
            t.Id != originalRecurringTransaction.Id &&
            t.Kind == originalRecurringTransaction.Kind &&
            t.StartDate == new DateOnly(2026, 2, 1));
        
        Assert.Equal(3, user.RecurringTransactions.Count);
        Assert.Equal(new DateOnly(2026,1,31), originalRecurringTransaction.EndDate);
        Assert.Equal(50m, originalRecurringTransaction.Amount);
        Assert.Equal("Gym", originalRecurringTransaction.Description);
        
        Assert.Equal(60m, newRecurringTransaction.Amount);
        Assert.Equal("Gym Membership", newRecurringTransaction.Description);
        Assert.Equal(16, newRecurringTransaction.ScheduledDayOfMonth);
        Assert.Equal(new DateOnly(2026, 2, 1), newRecurringTransaction.StartDate);
        Assert.Null(newRecurringTransaction.EndDate);
    }

    [Fact]
    public void ReplaceRecurringTransaction_ShouldNotMutateExistingTransaction_WhenReplacementIsInvalid()
    {
        // Arrange
        var user = CreateValidUser();

        var originalRecurringTransaction = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            "Gym",
            50m,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            new DateOnly(2026, 1, 10),
            null,
            15);

        user.AddRecurringTransaction(originalRecurringTransaction);

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            user.ReplaceRecurringTransaction(
                originalRecurringTransaction.Id,
                "",
                60m,
                16,
                new DateOnly(2026, 2, 1)));

        // Assert
        Assert.Equal("description", exception.ParamName);

        Assert.Null(originalRecurringTransaction.EndDate);
        Assert.Equal("Gym", originalRecurringTransaction.Description);
        Assert.Equal(50m, originalRecurringTransaction.Amount);
        Assert.Equal(new DateOnly(2026, 1, 10), originalRecurringTransaction.StartDate);
        Assert.Equal(15, originalRecurringTransaction.ScheduledDayOfMonth);

        Assert.Equal(2, user.RecurringTransactions.Count);
        Assert.DoesNotContain(user.RecurringTransactions, t =>
            t.Id != originalRecurringTransaction.Id &&
            t.Kind == RecurringTransactionKind.Expense &&
            t.StartDate == new DateOnly(2026, 2, 1));
    }

    [Fact]
    public void ReplaceRecurringTransaction_ShouldThrow_WhenRecurringTransactionDoesNotExist()
    {
        // Arrange
        var user = CreateValidUser();
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => user.ReplaceRecurringTransaction(
            Guid.NewGuid(),
            "Description",
            10m,
            1,
            new DateOnly(2026, 2, 1)));
        
        // Assert
        Assert.Equal("Recurring transaction was not found.", exception.Message);
        Assert.Single(user.RecurringTransactions);
    }
    
    [Fact]
    public void ReplaceRecurringTransaction_ShouldThrow_WhenReplacementStartDateIsBeforeExistingStartDate()
    {
        // Arrange
        var user = CreateValidUser();

        var originalRecurringTransaction = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            "Gym",
            50m,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            new DateOnly(2026, 2, 1),
            null,
            1);
        
        user.AddRecurringTransaction(originalRecurringTransaction);
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => user.ReplaceRecurringTransaction(originalRecurringTransaction.Id, "Gym Membership", 60m, 16, new DateOnly(2026, 1, 20)));

        //Assert
        Assert.Equal("Replacement start date must be after the existing transaction start date.", exception.Message);
    }
    
    [Fact]
    public void ReplaceRecurringTransaction_ShouldThrow_WhenReplacementStartDateIsEqualToExistingStartDate()
    {
        // Arrange
        var user = CreateValidUser();

        var originalRecurringTransaction = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            "Gym",
            50m,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            new DateOnly(2026, 2, 1),
            null,
            1);
        
        user.AddRecurringTransaction(originalRecurringTransaction);
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => user.ReplaceRecurringTransaction(originalRecurringTransaction.Id, "Gym Membership", 60m, 16, new DateOnly(2026, 2, 1)));

        //Assert
        Assert.Equal("Replacement start date must be after the existing transaction start date.", exception.Message);
        Assert.Null(originalRecurringTransaction.EndDate);
    }

    [Fact]
    public void ReplaceRecurringTransaction_ShouldThrow_WhenTransactionHasAlreadyEnded()
    {
        // Arrange
        var user = CreateValidUser();
        
        var originalRecurringTransaction = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            "Gym",
            50m,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            new DateOnly(2026, 2, 1),
            null,
            1);
        
        user.AddRecurringTransaction(originalRecurringTransaction);
        user.EndRecurringTransaction(originalRecurringTransaction.Id, new DateOnly(2026, 2, 2));

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => user.ReplaceRecurringTransaction(originalRecurringTransaction.Id, "Gym", 55, 1, new DateOnly(2026, 2, 10)));
        
        // Assert
        Assert.Equal("Cannot replace a transaction that has already ended.", exception.Message);
    }

    [Fact]
    public void AddRecurringTransaction_ShouldAddExpenseTransaction()
    {
        // Arrange
        var user = CreateValidUser();
        var recurringTransaction = CreateValidRecurringExpense(user.Id);
        
        // Act
        user.AddRecurringTransaction(recurringTransaction);
        
        //Assert
        var transaction = user.RecurringTransactions.Single(t => t.Id == recurringTransaction.Id);
        
        Assert.Equal(recurringTransaction.Id, transaction.Id);
        Assert.Equal(user.Id, transaction.UserId);
        Assert.Equal(recurringTransaction.Description, transaction.Description);
        Assert.Equal(recurringTransaction.Amount, transaction.Amount);
        Assert.Equal(recurringTransaction.Type, transaction.Type);
        Assert.Equal(recurringTransaction.StartDate, transaction.StartDate);
        Assert.Equal(recurringTransaction.EndDate, transaction.EndDate);
        Assert.Equal(recurringTransaction.ScheduledDayOfMonth, transaction.ScheduledDayOfMonth);
        Assert.Equal(recurringTransaction.Kind, transaction.Kind);
        Assert.Equal(2, user.RecurringTransactions.Count);
    }

    [Fact]
    public void AddRecurringTransaction_ShouldThrow_WhenTransactionBelongsToDifferentUser()
    {
        // Arrange
        var user = CreateValidUser();
        var recurringTransaction = CreateValidRecurringExpense(Guid.NewGuid());
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => user.AddRecurringTransaction(recurringTransaction));
        
        // Assert
        Assert.Equal("Recurring transaction must belong to this user.", exception.Message);
    }

    [Fact]
    public void AddRecurringTransaction_ShouldThrow_WhenStartDateIsBeforeUserStartDate()
    {
        // Arrange
        var user = CreateValidUser();
        var recurringTransaction = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            "Gym",
            50m,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            new DateOnly(2025, 12, 1),
            null,
            1);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => user.AddRecurringTransaction(recurringTransaction));
        
        // Assert
        Assert.Equal("Recurring transaction cannot start before user start date.", exception.Message);
    }

    [Fact]
    public void AddRecurringTransaction_ShouldThrow_WhenSalaryOverlapsExistingSalary()
    {
        // Arrange
        var user = CreateValidUser();

        var overlappingSalary = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            "Salary",
            2000m,
            TransactionType.Income,
            RecurringTransactionKind.Salary,
            new DateOnly(2026, 1, 20),
            null,
            1);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => user.AddRecurringTransaction(overlappingSalary));
        
        // Assert
        Assert.Equal("User cannot have overlapping salary transactions.", exception.Message);
        Assert.Single(user.RecurringTransactions);
    }

    [Fact]
    public void EndRecurringTransaction_ShouldSetEndDate_ForExpenseTransaction()
    {
        // Arrange
        var user = CreateValidUser();
        var recurringTransaction = CreateValidRecurringExpense(user.Id);
        user.AddRecurringTransaction(recurringTransaction);
        
        // Act
        user.EndRecurringTransaction(recurringTransaction.Id, new DateOnly(2026, 2, 1));
        
        // Assert
        var transaction = user.RecurringTransactions.Single(t => t.Id == recurringTransaction.Id);
        Assert.Equal(new DateOnly(2026,2,1), transaction.EndDate);
    }
    
    [Fact]
    public void EndRecurringTransaction_ShouldThrow_WhenTransactionDoesNotExist()
    {
        // Arrange
        var user = CreateValidUser();
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => user.EndRecurringTransaction(Guid.NewGuid(), new DateOnly(2026, 2, 1)));
        // Assert
        Assert.Equal("Recurring transaction was not found.", exception.Message);
        Assert.Single(user.RecurringTransactions);
    }
    
    [Fact]
    public void EndRecurringTransaction_ShouldThrow_WhenTransactionIsSalary()
    {
        // Arrange
        var user = CreateValidUser();
        var salaryTransaction = user.RecurringTransactions.Single();
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => user.EndRecurringTransaction(salaryTransaction.Id, new DateOnly(2026, 2, 1)));
        
        // Assert
        Assert.Equal("Salary transactions cannot be ended directly. Use replacement instead.", exception.Message);
        Assert.Null(salaryTransaction.EndDate);
    }
    
    [Fact]
    public void EndRecurringTransaction_ShouldThrow_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        var user = CreateValidUser();
        var recurringTransaction = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            "Gym",
            50m,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            new DateOnly(2026, 2, 1),
            null,
            1);
        user.AddRecurringTransaction(recurringTransaction);
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>  user.EndRecurringTransaction(recurringTransaction.Id, new DateOnly(2026, 1, 20)));
        
        // Assert
        Assert.Equal("End date cannot be before the transaction start date.", exception.Message);
        Assert.Null(recurringTransaction.EndDate);
    }
        
    [Fact]
    public void GetProjectedSavingsForCurrentCycle_ShouldReturnInitialBalance_WhenTodayIsBeforeFirstSalaryDate()
    {
        // Arrange
        var user = CreateValidUser();
        
        // Act
        var balance = user.GetProjectedSavingsForCurrentCycle(new DateOnly(2026, 1, 2));
        
        // Assert
        Assert.Equal(1000m, balance);
    }    
         
    [Fact]
    public void GetProjectedSavingsForCurrentCycle_ShouldIncludeCurrentCycleSalary_WhenTodayIsOnOrAfterSalaryDate()
    {
        // Arrange
        var user = CreateValidUser();
        
        // Act
        var balance = user.GetProjectedSavingsForCurrentCycle(new DateOnly(2026, 1, 26));

        // Assert
        Assert.Equal(3000m, balance);
    }
        
    [Fact]
    public void GetProjectedSavingsForCurrentCycle_ShouldIncludeOneOffExpenseWithinCurrentCycle()
    {
        // Arrange
        var user = CreateValidUser();
        var oneOffTransaction = new OneOffTransaction(Guid.NewGuid(), user.Id, "Food", 100m, TransactionType.Expense, new DateOnly(2026, 2, 1));
        user.AddOneOffTransaction(oneOffTransaction);
        
        // Act
        var balance = user.GetProjectedSavingsForCurrentCycle(new DateOnly(2026, 1, 26));
        
        // Assert
        Assert.Equal(2900m, balance);
    }

    [Fact]
    public void GetProjectedSavingsForCurrentCycle_ShouldIgnoreOneOffExpenseAfterCurrentCycleEnd()
    {
        // Arrange
        var user = CreateValidUser();
        var oneOffTransaction = new OneOffTransaction(Guid.NewGuid(), user.Id, "Food", 100m, TransactionType.Expense, new DateOnly(2026, 2, 26));
        user.AddOneOffTransaction(oneOffTransaction);
        
        // Act
        var balance = user.GetProjectedSavingsForCurrentCycle(new DateOnly(2026, 1, 26));
        
        // Assert
        Assert.Equal(3000m, balance);
    }
        
    [Fact]
    public void GetProjectedSavingsForCurrentCycle_ShouldThrow_WhenTodayIsBeforeUserStartDate()
    {
        // Arrange
        var user = CreateValidUser();
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => user.GetProjectedSavingsForCurrentCycle(new DateOnly(2025, 1, 1)));
        
        // Assert
        Assert.Equal("Cannot project balance before user start date.", exception.Message);
    }    
    
    /*
     * ALL TESTS BELOW GENERATED THROUGH CODEX
     */
    
    [Fact]
    public void AddOneOffTransaction_ShouldThrow_WhenTransactionIsNull()
    {
        // Arrange
        var user = CreateValidUser();
    
        // Act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            user.AddOneOffTransaction(null!));
    
        // Assert
        Assert.Equal("oneOffTransaction", exception.ParamName);
    }

    [Fact]
    public void EditOneOffTransaction_ShouldThrow_WhenTransactionDoesNotExist()
    {
        // Arrange
        var user = CreateValidUser();
    
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
            user.EditOneOffTransaction(Guid.NewGuid(), "Updated", 100m));
    
        // Assert
        Assert.Equal("Transaction was not found.", exception.Message);
    }
    
    [Fact]
    public void EditOneOffTransaction_ShouldThrow_WhenDescriptionIsBlank()
    {
        // Arrange
        var user = CreateValidUser();
    
        var oneOffTransaction = new OneOffTransaction(
            Guid.NewGuid(),
            user.Id,
            "Bonus",
            500m,
            TransactionType.Income,
            new DateOnly(2026, 1, 10));
    
        user.AddOneOffTransaction(oneOffTransaction);
    
        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            user.EditOneOffTransaction(oneOffTransaction.Id, "", 600m));
    
        // Assert
        Assert.Equal("description", exception.ParamName);
        Assert.Equal("Bonus", oneOffTransaction.Description);
        Assert.Equal(500m, oneOffTransaction.Amount);
    }
    
    [Fact]
    public void EditOneOffTransaction_ShouldThrow_WhenAmountIsZero()
    {
        // Arrange
        var user = CreateValidUser();
    
        var oneOffTransaction = new OneOffTransaction(
            Guid.NewGuid(),
            user.Id,
            "Bonus",
            500m,
            TransactionType.Income,
            new DateOnly(2026, 1, 10));
    
        user.AddOneOffTransaction(oneOffTransaction);
    
        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            user.EditOneOffTransaction(oneOffTransaction.Id, "Updated bonus", 0m));
    
        // Assert
        Assert.Equal("amount", exception.ParamName);
        Assert.Equal("Bonus", oneOffTransaction.Description);
        Assert.Equal(500m, oneOffTransaction.Amount);
    }
    
    [Fact]
    public void DeleteOneOffTransaction_ShouldRemoveTransaction()
    {
        // Arrange
        var user = CreateValidUser();
    
        var oneOffTransaction = new OneOffTransaction(
            Guid.NewGuid(),
            user.Id,
            "Bonus",
            500m,
            TransactionType.Income,
            new DateOnly(2026, 1, 10));
    
        user.AddOneOffTransaction(oneOffTransaction);
    
        // Act
        user.DeleteOneOffTransaction(oneOffTransaction.Id);
    
        // Assert
        Assert.Empty(user.OneOffTransactions);
    }
    
    [Fact]
    public void DeleteOneOffTransaction_ShouldThrow_WhenTransactionDoesNotExist()
    {
        // Arrange
        var user = CreateValidUser();
    
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
            user.DeleteOneOffTransaction(Guid.NewGuid()));
    
        // Assert
        Assert.Equal("Transaction was not found.", exception.Message);
    }
    
    [Fact]
    public void AddRecurringTransaction_ShouldThrow_WhenTransactionIsNull()
    {
        // Arrange
        var user = CreateValidUser();
    
        // Act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            user.AddRecurringTransaction(null!));
    
        // Assert
        Assert.Equal("recurringTransaction", exception.ParamName);
    }
    
    [Fact]
    public void EndRecurringTransaction_ShouldThrow_WhenTransactionHasAlreadyEnded()
    {
        // Arrange
        var user = CreateValidUser();
        var recurringTransaction = CreateValidRecurringExpense(user.Id);
    
        user.AddRecurringTransaction(recurringTransaction);
        user.EndRecurringTransaction(recurringTransaction.Id, new DateOnly(2026, 2, 1));
    
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
            user.EndRecurringTransaction(recurringTransaction.Id, new DateOnly(2026, 2, 2)));
    
        // Assert
        Assert.Equal("Recurring transaction has already ended.", exception.Message);
        Assert.Equal(new DateOnly(2026, 2, 1), recurringTransaction.EndDate);
    }
    
    [Fact]
    public void GetBalanceOn_ShouldThrow_WhenTargetDateIsBeforeUserStartDate()
    {
        // Arrange
        var user = CreateValidUser();
    
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
            user.GetBalanceOn(new DateOnly(2025, 12, 31)));
    
        // Assert
        Assert.Equal("Cannot calculate balance before user start date.", exception.Message);
    }
    
    [Fact]
    public void GetBalanceOn_ShouldIncludeRecurringExpenseOccurrences()
    {
        // Arrange
        var user = CreateValidUser();
        var recurringTransaction = CreateValidRecurringExpense(user.Id);
    
        user.AddRecurringTransaction(recurringTransaction);
    
        // Act
        var balance = user.GetBalanceOn(new DateOnly(2026, 2, 16));
    
        // Assert
        Assert.Equal(2900m, balance);
    }
    
    [Fact]
    public void GetBalanceOn_ShouldStopCountingRecurringExpenseAfterEndDate()
    {
        // Arrange
        var user = CreateValidUser();
        var recurringTransaction = CreateValidRecurringExpense(user.Id);
    
        user.AddRecurringTransaction(recurringTransaction);
        user.EndRecurringTransaction(recurringTransaction.Id, new DateOnly(2026, 2, 1));
    
        // Act
        var balance = user.GetBalanceOn(new DateOnly(2026, 3, 31));
    
        // Assert
        Assert.Equal(6950m, balance);
    }
    
    [Fact]
    public void GetBalanceOn_ShouldUseReplacementSalaryForFutureBalance_AndKeepHistoricalSalary()
    {
        // Arrange
        var user = CreateValidUser();
        var originalSalary = user.RecurringTransactions.Single(t => t.Kind == RecurringTransactionKind.Salary);
    
        user.ReplaceRecurringTransaction(
            originalSalary.Id,
            "Salary",
            3000m,
            25,
            new DateOnly(2026, 3, 1));
    
        // Act
        var historicalBalance = user.GetBalanceOn(new DateOnly(2026, 2, 28));
        var futureBalance = user.GetBalanceOn(new DateOnly(2026, 3, 25));
    
        // Assert
        Assert.Equal(5000m, historicalBalance);
        Assert.Equal(8000m, futureBalance);
    
        Assert.Equal(new DateOnly(2026, 2, 28), originalSalary.EndDate);
        Assert.Equal(2, user.RecurringTransactions.Count(t => t.Kind == RecurringTransactionKind.Salary));
    }
    
    [Fact]
    public void GetCurrentCycleEndDate_ShouldReturnDayBeforeSalaryDate_WhenTodayIsBeforeSalaryDate()
    {
        // Arrange
        var user = CreateValidUser();
    
        // Act
        var cycleEndDate = user.GetCurrentCycleEndDate(new DateOnly(2026, 1, 10));
    
        // Assert
        Assert.Equal(new DateOnly(2026, 1, 24), cycleEndDate);
    }
    
    [Fact]
    public void GetCurrentCycleEndDate_ShouldReturnDayBeforeNextSalaryDate_WhenTodayIsOnSalaryDate()
    {
        // Arrange
        var user = CreateValidUser();
    
        // Act
        var cycleEndDate = user.GetCurrentCycleEndDate(new DateOnly(2026, 1, 25));
    
        // Assert
        Assert.Equal(new DateOnly(2026, 2, 24), cycleEndDate);
    }
    
    [Fact]
    public void GetCurrentCycleEndDate_ShouldReturnDayBeforeNextSalaryDate_WhenTodayIsAfterSalaryDate()
    {
        // Arrange
        var user = CreateValidUser();
    
        // Act
        var cycleEndDate = user.GetCurrentCycleEndDate(new DateOnly(2026, 1, 26));
    
        // Assert
        Assert.Equal(new DateOnly(2026, 2, 24), cycleEndDate);
    }
    
    [Fact]
    public void GetCurrentCycleEndDate_ShouldClampSalaryDay_WhenMonthIsShorterThanScheduledDay()
    {
        // Arrange
        var user = User.Create(
            Guid.NewGuid(),
            "John",
            1000m,
            new DateOnly(2026, 1, 1),
            2000m,
            31);
    
        // Act
        var cycleEndDate = user.GetCurrentCycleEndDate(new DateOnly(2026, 2, 1));
    
        // Assert
        Assert.Equal(new DateOnly(2026, 2, 27), cycleEndDate);
    }
    
    [Fact]
    public void GetCurrentCycleEndDate_ShouldThrow_WhenTodayIsBeforeUserStartDate()
    {
        // Arrange
        var user = CreateValidUser();
    
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
            user.GetCurrentCycleEndDate(new DateOnly(2025, 12, 31)));
    
        // Assert
        Assert.Equal("Cannot determine cycle end date before user start date.", exception.Message);
    }
    
    [Fact]
    public void GetProjectedSavingsForCurrentCycle_ShouldUseActiveReplacementSalary()
    {
        // Arrange
        var user = CreateValidUser();
        var originalSalary = user.RecurringTransactions.Single(t => t.Kind == RecurringTransactionKind.Salary);
    
        user.ReplaceRecurringTransaction(
            originalSalary.Id,
            "Salary",
            3000m,
            25,
            new DateOnly(2026, 3, 1));
    
        var oneOffTransaction = new OneOffTransaction(
            Guid.NewGuid(),
            user.Id,
            "Food",
            100m,
            TransactionType.Expense,
            new DateOnly(2026, 4, 5));
    
        user.AddOneOffTransaction(oneOffTransaction);
    
        // Act
        var projectedSavings = user.GetProjectedSavingsForCurrentCycle(new DateOnly(2026, 3, 25));
    
        // Assert
        Assert.Equal(7900m, projectedSavings);
    }
}