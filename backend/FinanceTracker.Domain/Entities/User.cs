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
        ArgumentNullException.ThrowIfNull(recurringTransaction);

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

    public decimal GetBalanceOn(DateOnly targetDate)
    {
        if (targetDate < StartDate)
            throw new InvalidOperationException("Cannot calculate balance before user start date.");

        var balance = InitialBalance;

        balance += CalculateOneOffTransactions(targetDate);
        balance += CalculateRecurringTransactions(targetDate);
        
        return balance;
    }

    private decimal CalculateOneOffTransactions(DateOnly targetDate)
    {
        decimal total = 0;

        foreach (var transaction in _oneOffTransactions)
        {
            if (transaction.Date > targetDate) continue;
            if (transaction.Type == TransactionType.Income)
            {
                total += transaction.Amount;
            }
            else
            {
                total -= transaction.Amount;
            }
        }
        return total;
    }

    private decimal CalculateRecurringTransactions(DateOnly targetDate)
    {
        decimal total = 0;

        foreach (var transaction in _recurringTransactions)
        {
            var occurrences = GetRecurringTransactionOccurrencesUpTo(transaction, targetDate);

            if (transaction.Type == TransactionType.Income)
            {
                total += transaction.Amount * occurrences;
            }
            else
            {
                total -= transaction.Amount * occurrences;
            }
        }

        return total;
    }

    private int GetRecurringTransactionOccurrencesUpTo(RecurringTransaction transaction, DateOnly targetDate)
    {
        var occurrences = 0;

        var currentMonth = new DateOnly(StartDate.Year, StartDate.Month, 1);
        var targetMonth = new DateOnly(targetDate.Year, targetDate.Month, 1);

        while (currentMonth <= targetMonth)
        {
            var daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
            var day = Math.Min(transaction.DayOfMonth, daysInMonth);

            var occurrenceDate = new DateOnly(currentMonth.Year, currentMonth.Month, day);

            if (occurrenceDate >= StartDate && occurrenceDate <= targetDate)
            {
                occurrences++;
            }

            currentMonth = currentMonth.AddMonths(1);
        }

        return occurrences;
    }
    
    public decimal GetProjectedSavingsForCurrentCycle(DateOnly today)
    {
        if (today < StartDate)
            throw new InvalidOperationException("Cannot project balance before user start date.");

        var salaryTransaction = GetSalaryTransaction();
        var cycleEndDate = CalculateCycleEndDate(today, salaryTransaction.DayOfMonth);

        return GetBalanceOn(cycleEndDate);
    }
    
    private RecurringTransaction GetSalaryTransaction()
    {
        var salaryTransaction = _recurringTransactions
            .SingleOrDefault(t => t.Kind == RecurringTransactionKind.Salary);

        return salaryTransaction ?? throw new InvalidOperationException("User must have a salary transaction.");
    }
    
    private DateOnly CalculateCycleEndDate(DateOnly today, int salaryDayOfMonth)
    {
        var salaryDateThisMonth = CreateValidDate(today.Year, today.Month, salaryDayOfMonth);

        if (today < salaryDateThisMonth)
        {
            return salaryDateThisMonth.AddDays(-1);
        }

        var salaryDateNextMonth = salaryDateThisMonth.AddMonths(1);

        return salaryDateNextMonth.AddDays(-1);
    }
    
    public DateOnly GetCurrentCycleEndDate(DateOnly today)
    {
        if (today < StartDate)
            throw new InvalidOperationException("Cannot determine cycle end date before user start date.");

        var salaryTransaction = GetSalaryTransaction();

        return CalculateCycleEndDate(today, salaryTransaction.DayOfMonth);
    }
    
    private DateOnly CreateValidDate(int year, int month, int dayOfMonth)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var validDay = Math.Min(dayOfMonth, daysInMonth);

        return new DateOnly(year, month, validDay);
    }
    
}