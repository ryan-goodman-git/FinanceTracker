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
            startDate,
            null,
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

            var overlappingSalaryExists = _recurringTransactions.Any(t =>
                t.Kind == RecurringTransactionKind.Salary &&
                DatesOverlap(t.StartDate, t.EndDate, recurringTransaction.StartDate, recurringTransaction.EndDate));

            if (overlappingSalaryExists)
                throw new InvalidOperationException("User cannot have overlapping salary transactions.");
        }

        if (recurringTransaction.Kind == RecurringTransactionKind.Expense)
        {
            if (recurringTransaction.Type != TransactionType.Expense)
                throw new InvalidOperationException("Expense recurring transaction must be expense.");
        }

        _recurringTransactions.Add(recurringTransaction);
    }
    
    private static bool DatesOverlap(DateOnly start1, DateOnly? end1, DateOnly start2, DateOnly? end2)
    {
        var effectiveEnd1 = end1 ?? DateOnly.MaxValue;
        var effectiveEnd2 = end2 ?? DateOnly.MaxValue;

        return start1 <= effectiveEnd2 && start2 <= effectiveEnd1;
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
    // Holds the total number of valid occurrences found
    var occurrences = 0;

    // Start from the first day of the month in which the transaction becomes active
    var evaluationMonth = new DateOnly(transaction.StartDate.Year, transaction.StartDate.Month, 1);

    // Represents the month we want to stop at (based on the target date)
    var targetMonth = new DateOnly(targetDate.Year, targetDate.Month, 1);

    // Loop month-by-month until we reach the target month
    while (evaluationMonth <= targetMonth)
    {
        // Get how many days exist in this specific month (handles Feb, leap years, etc.)
        var daysInMonth = DateTime.DaysInMonth(evaluationMonth.Year, evaluationMonth.Month);

        // Ensure the scheduled day is valid for this month
        // e.g. if scheduled for 31st but month has 30 days → use 30
        var day = Math.Min(transaction.ScheduledDayOfMonth, daysInMonth);

        // Build the actual date this transaction would occur in this month
        var occurrenceDate = new DateOnly(evaluationMonth.Year, evaluationMonth.Month, day);

        // Check the transaction has actually started by this occurrence
        var isOnOrAfterStartDate = occurrenceDate >= transaction.StartDate;

        // Check the transaction has not ended yet (or is still active)
        var isOnOrBeforeEndDate = !transaction.EndDate.HasValue || occurrenceDate <= transaction.EndDate.Value;

        // Ensure we do not count occurrences beyond the requested target date
        var isOnOrBeforeTargetDate = occurrenceDate <= targetDate;

        // Only count the occurrence if it passes all validity checks
        if (isOnOrAfterStartDate && isOnOrBeforeEndDate && isOnOrBeforeTargetDate)
        {
            occurrences++;
        }
        
        evaluationMonth = evaluationMonth.AddMonths(1);
    }
    return occurrences;
}
    
    public decimal GetProjectedSavingsForCurrentCycle(DateOnly today)
    {
        if (today < StartDate)
            throw new InvalidOperationException("Cannot project balance before user start date.");

        var salaryTransaction = GetActiveSalaryTransactionOn(today);
        var cycleEndDate = CalculateCycleEndDate(today, salaryTransaction.ScheduledDayOfMonth);

        return GetBalanceOn(cycleEndDate);
    }
    
    private RecurringTransaction GetActiveSalaryTransactionOn(DateOnly date)
    {
        var salaryTransaction = _recurringTransactions.SingleOrDefault(t =>
            t.Kind == RecurringTransactionKind.Salary &&
            t.StartDate <= date &&
            (!t.EndDate.HasValue || t.EndDate.Value >= date));

        return salaryTransaction 
               ?? throw new InvalidOperationException("User must have an active salary on this date.");
    }
    
    private DateOnly CalculateCycleEndDate(DateOnly today, int salaryDayOfMonth)
    {
        var salaryDateThisMonth = CreateValidDate(today.Year, today.Month, salaryDayOfMonth);

        if (today < salaryDateThisMonth)
        {
            return salaryDateThisMonth.AddDays(-1);
        }

        var nextMonth = today.AddMonths(1);
        var salaryDateNextMonth = CreateValidDate(nextMonth.Year, nextMonth.Month, salaryDayOfMonth);

        return salaryDateNextMonth.AddDays(-1);
    }
    
    public DateOnly GetCurrentCycleEndDate(DateOnly today)
    {
        if (today < StartDate)
            throw new InvalidOperationException("Cannot determine cycle end date before user start date.");

        var salaryTransaction = GetActiveSalaryTransactionOn(today);

        return CalculateCycleEndDate(today, salaryTransaction.ScheduledDayOfMonth);
    }
    
    private DateOnly CreateValidDate(int year, int month, int dayOfMonth)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var validDay = Math.Min(dayOfMonth, daysInMonth);

        return new DateOnly(year, month, validDay);
    }
    
}