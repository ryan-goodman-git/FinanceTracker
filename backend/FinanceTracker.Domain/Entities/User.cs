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

    /// <summary>
    /// Private constructor that sets the core state of the user.
    /// Called by the factory method after validating required values.
    /// </summary>
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
    /// Factory method used to create a User in a valid state.
    /// A user must be created with an initial salary recurring transaction.
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
    /// Adds a recurring transaction after enforcing aggregate rules:
    /// correct ownership, valid start date, valid type/kind combination,
    /// and no overlapping salary date ranges.
    /// </summary>
    public void AddRecurringTransaction(RecurringTransaction recurringTransaction)
    {
        ValidateRecurringTransactionCanBeAdded(recurringTransaction);
        _recurringTransactions.Add(recurringTransaction);
    }

    /// <summary>
    /// Ends a non-salary recurring transaction on the supplied end date.
    /// Salary changes must go through replacement so history is preserved.
    /// </summary>
    public void EndRecurringTransaction(Guid recurringTransactionId, DateOnly endDate)
    {
        var recurringTransaction = _recurringTransactions.SingleOrDefault(t => t.Id == recurringTransactionId);

        if (recurringTransaction is null)
            throw new InvalidOperationException("Recurring transaction was not found.");

        if (recurringTransaction.Kind == RecurringTransactionKind.Salary)
            throw new InvalidOperationException("Salary transactions cannot be ended directly. Use replacement instead.");

        if (endDate < recurringTransaction.StartDate)
            throw new InvalidOperationException("End date cannot be before the transaction start date.");

        recurringTransaction.EndOn(endDate);
    }

    /// <summary>
    /// Replaces an existing recurring transaction by ending the old version
    /// the day before the supplied replacement start date and creating a new version.
    /// This preserves financial history instead of overwriting the original record.
    /// </summary>
    public RecurringTransaction ReplaceRecurringTransaction(
        Guid recurringTransactionId,
        string description,
        decimal amount,
        int scheduledDayOfMonth,
        DateOnly replacementStartDate)
    {
        var existing = _recurringTransactions.SingleOrDefault(t => t.Id == recurringTransactionId);

        if (existing is null)
            throw new InvalidOperationException("Recurring transaction was not found.");

        if (replacementStartDate <= existing.StartDate)
            throw new InvalidOperationException(
                "Replacement start date must be after the existing transaction start date.");

        if (existing.EndDate.HasValue)
            throw new InvalidOperationException("Cannot replace a transaction that has already ended.");
        
        var replacement = new RecurringTransaction(
            Guid.NewGuid(),
            Id,
            description,
            amount,
            existing.Type,
            existing.Kind,
            replacementStartDate,
            null,
            scheduledDayOfMonth);
        
        ValidateRecurringTransactionCanBeAdded(replacement, existing.Id);
        
        existing.EndOn(replacementStartDate.AddDays(-1));
        _recurringTransactions.Add(replacement);
        
        return replacement;
    }
    
    /// <summary>
    /// Validates whether a recurring transaction can be added to this user
    /// without mutating aggregate state.
    /// Optionally ignores one existing transaction during overlap checks,
    /// which is useful during replacement.
    /// </summary>
    private void ValidateRecurringTransactionCanBeAdded(
        RecurringTransaction recurringTransaction,
        Guid? recurringTransactionIdToIgnore = null)
    {
        ArgumentNullException.ThrowIfNull(recurringTransaction);

        if (recurringTransaction.UserId != Id)
            throw new InvalidOperationException("Recurring transaction must belong to this user.");

        if (recurringTransaction.StartDate < StartDate)
            throw new InvalidOperationException("Recurring transaction cannot start before user start date.");

        if (recurringTransaction.Kind == RecurringTransactionKind.Salary)
        {
            if (recurringTransaction.Type != TransactionType.Income)
                throw new InvalidOperationException("Salary transaction must be income.");

            var overlappingSalaryExists = _recurringTransactions.Any(t =>
                t.Id != recurringTransactionIdToIgnore &&
                t.Kind == RecurringTransactionKind.Salary &&
                DatesOverlap(t.StartDate, t.EndDate, recurringTransaction.StartDate, recurringTransaction.EndDate));

            if (overlappingSalaryExists)
                throw new InvalidOperationException("User cannot have overlapping salary transactions.");
        }

        if (recurringTransaction.Kind == RecurringTransactionKind.Expense &&
            recurringTransaction.Type != TransactionType.Expense)
        {
            throw new InvalidOperationException("Expense recurring transaction must be expense.");
        }
    }

    /// <summary>
    /// Adds a one-off transaction while ensuring it belongs to this user
    /// and does not occur before the user's start date.
    /// </summary>
    public void AddOneOffTransaction(OneOffTransaction oneOffTransaction)
    {
        ArgumentNullException.ThrowIfNull(oneOffTransaction);

        if (oneOffTransaction.UserId != Id)
            throw new InvalidOperationException("Transaction must belong to this user.");

        if (oneOffTransaction.Date < StartDate)
            throw new InvalidOperationException("Transaction cannot be before user start date.");

        _oneOffTransactions.Add(oneOffTransaction);
    }

    /// <summary>
    /// Updates the description and amount of an existing one-off transaction.
    /// The transaction must exist and remain owned by this aggregate.
    /// </summary>
    public void EditOneOffTransaction(Guid oneOffTransactionId, string description, decimal amount)
    {
        var transaction = _oneOffTransactions.SingleOrDefault(t => t.Id == oneOffTransactionId);

        if (transaction is null)
            throw new InvalidOperationException("Transaction was not found.");

        transaction.UpdateDetails(description, amount);
    }

    /// <summary>
    /// Removes an existing one-off transaction from the user.
    /// The transaction must exist before it can be deleted.
    /// </summary>
    public void DeleteOneOffTransaction(Guid oneOffTransactionId)
    {
        var transaction = _oneOffTransactions.SingleOrDefault(t => t.Id == oneOffTransactionId);

        if (transaction is null)
            throw new InvalidOperationException("Transaction was not found.");

        _oneOffTransactions.Remove(transaction);
    }

    /// <summary>
    /// Calculates the user's balance on a given date by combining:
    /// initial balance, valid one-off transactions, and valid recurring occurrences up to that date.
    /// </summary>
    public decimal GetBalanceOn(DateOnly targetDate)
    {
        if (targetDate < StartDate)
            throw new InvalidOperationException("Cannot calculate balance before user start date.");

        var balance = InitialBalance;

        balance += CalculateOneOffTotalUpTo(targetDate);
        balance += CalculateRecurringTotalUpTo(targetDate);

        return balance;
    }

    /// <summary>
    /// Calculates the projected balance at the end of the current salary cycle.
    /// Uses the salary that is active on the supplied date.
    /// </summary>
    public decimal GetProjectedSavingsForCurrentCycle(DateOnly today)
    {
        if (today < StartDate)
            throw new InvalidOperationException("Cannot project balance before user start date.");

        var cycleEndDate = GetCurrentCycleEndDate(today);

        return GetBalanceOn(cycleEndDate);
    }

    /// <summary>
    /// Returns the final date of the current salary cycle based on the salary
    /// active on the supplied date.
    /// </summary>
    public DateOnly GetCurrentCycleEndDate(DateOnly today)
    {
        if (today < StartDate)
            throw new InvalidOperationException("Cannot determine cycle end date before user start date.");

        var salaryTransaction = GetActiveSalaryTransactionOn(today);

        return CalculateCycleEndDate(today, salaryTransaction.ScheduledDayOfMonth);
    }

    /// <summary>
    /// Calculates the signed total of all one-off transactions up to and including the target date.
    /// Income adds to the balance; expense reduces it.
    /// </summary>
    private decimal CalculateOneOffTotalUpTo(DateOnly targetDate)
    {
        decimal total = 0;

        foreach (var transaction in _oneOffTransactions)
        {
            if (transaction.Date > targetDate)
                continue;

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

    /// <summary>
    /// Calculates the signed total of all recurring transaction occurrences
    /// up to and including the target date.
    /// </summary>
    private decimal CalculateRecurringTotalUpTo(DateOnly targetDate)
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

    /// <summary>
    /// Counts how many valid monthly occurrences of a recurring transaction
    /// exist up to and including the target date.
    /// </summary>
    private int GetRecurringTransactionOccurrencesUpTo(RecurringTransaction transaction, DateOnly targetDate)
    {
        var occurrences = 0;

        var evaluationMonth = new DateOnly(transaction.StartDate.Year, transaction.StartDate.Month, 1);
        var targetMonth = new DateOnly(targetDate.Year, targetDate.Month, 1);

        while (evaluationMonth <= targetMonth)
        {
            var daysInMonth = DateTime.DaysInMonth(evaluationMonth.Year, evaluationMonth.Month);
            var day = Math.Min(transaction.ScheduledDayOfMonth, daysInMonth);
            var occurrenceDate = new DateOnly(evaluationMonth.Year, evaluationMonth.Month, day);

            var isOnOrAfterStartDate = occurrenceDate >= transaction.StartDate;
            var isOnOrBeforeEndDate = !transaction.EndDate.HasValue || occurrenceDate <= transaction.EndDate.Value;
            var isOnOrBeforeTargetDate = occurrenceDate <= targetDate;

            if (isOnOrAfterStartDate && isOnOrBeforeEndDate && isOnOrBeforeTargetDate)
            {
                occurrences++;
            }

            evaluationMonth = evaluationMonth.AddMonths(1);
        }

        return occurrences;
    }

    /// <summary>
    /// Finds the salary transaction that is active on the supplied date.
    /// Cycle-based calculations depend on this to determine which salary schedule applies.
    /// </summary>
    private RecurringTransaction GetActiveSalaryTransactionOn(DateOnly date)
    {
        var salaryTransaction = _recurringTransactions.SingleOrDefault(t =>
            t.Kind == RecurringTransactionKind.Salary &&
            t.StartDate <= date &&
            (!t.EndDate.HasValue || t.EndDate.Value >= date));

        return salaryTransaction
               ?? throw new InvalidOperationException("User must have an active salary on this date.");
    }

    /// <summary>
    /// Calculates the end date of the current salary cycle.
    /// The cycle ends on the day before the next salary date.
    /// </summary>
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

    /// <summary>
    /// Creates a valid date for the supplied year, month, and scheduled day.
    /// If the scheduled day does not exist in that month, it is clamped to the month's last day.
    /// </summary>
    private DateOnly CreateValidDate(int year, int month, int dayOfMonth)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var validDay = Math.Min(dayOfMonth, daysInMonth);

        return new DateOnly(year, month, validDay);
    }

    /// <summary>
    /// Returns true when two date ranges overlap.
    /// A null end date is treated as open-ended.
    /// </summary>
    private static bool DatesOverlap(DateOnly start1, DateOnly? end1, DateOnly start2, DateOnly? end2)
    {
        var effectiveEnd1 = end1 ?? DateOnly.MaxValue;
        var effectiveEnd2 = end2 ?? DateOnly.MaxValue;

        return start1 <= effectiveEnd2 && start2 <= effectiveEnd1;
    }
}