using System.Linq;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal InitialBalance { get; private set; }
    public DateOnly StartDate { get; private set; }

    private readonly List<RecurringTransaction> _recurringTransactions = new();
    public IReadOnlyCollection<RecurringTransaction> RecurringTransactions => _recurringTransactions;

    private readonly List<OneOffTransaction> _oneOffTransactions = new();
    public IReadOnlyCollection<OneOffTransaction> OneOffTransactions => _oneOffTransactions;

    private User()
    {
    }

    private User(Guid id, string name, decimal initialBalance, DateOnly startDate)
    {
        if (id == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("User name is required.", nameof(name));

        Id = id;
        Name = name.Trim();
        InitialBalance = initialBalance;
        StartDate = startDate;
    }
    /// <summary>
    /// Factory method (static) used to create a User in a valid state.
    /// Called on the class (no instance exists yet) and ensures a required salary is created.
    /// </summary>
    public static User Create(
        Guid id,
        string name,
        decimal initialBalance,
        DateOnly startDate,
        decimal salaryAmount,
        int salaryDayOfMonth)
    {
        var user = new User(id, name, initialBalance, startDate);

        var salary = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            "Salary",
            salaryAmount,
            TransactionType.Income,
            RecurringTransactionKind.Salary,
            salaryDayOfMonth);

        user.AddRecurringTransaction(salary);

        return user;
    }
    
    /// <summary>
    /// Adds a recurring transaction while enforcing aggregate rules
    /// (correct user, single salary, valid type/kind combination).
    /// </summary>
    public void AddRecurringTransaction(RecurringTransaction recurringTransaction)
    {
        if (recurringTransaction is null)
            throw new ArgumentNullException(nameof(recurringTransaction));

        if (recurringTransaction.UserId != Id)
            throw new InvalidOperationException("Recurring transaction must belong to this user.");

        if (recurringTransaction.Kind == RecurringTransactionKind.Salary)
        {
            if (recurringTransaction.Type != TransactionType.Income)
                throw new InvalidOperationException("Salary transaction must be income.");

            var salaryAlreadyExists = _recurringTransactions.Any(t => t.Kind == RecurringTransactionKind.Salary);
            if (salaryAlreadyExists)
                throw new InvalidOperationException("User can only have one salary transaction.");
        }

        if (recurringTransaction.Kind == RecurringTransactionKind.Expense)
        {
            if (recurringTransaction.Type != TransactionType.Expense)
                throw new InvalidOperationException("Expense recurring transaction must be expense.");
        }

        _recurringTransactions.Add(recurringTransaction);
    }
    /// <summary>
    /// Adds a one-off transaction while ensuring it belongs to this user
    /// and does not occur before the user's start date.
    /// </summary>
    public void AddOneOffTransaction(OneOffTransaction oneOffTransaction)
    {					
        if (oneOffTransaction is null)
            throw new ArgumentNullException(nameof(oneOffTransaction));
        
        if (oneOffTransaction.UserId != Id)
            throw new InvalidOperationException("Transaction must belong to this user.");
        
        if (oneOffTransaction.Date < StartDate)
            throw new InvalidOperationException("Transaction cannot be before user start date.");
        
        _oneOffTransactions.Add(oneOffTransaction);
     }
}