using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Entities;

public class RecurringTransaction
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public RecurringTransactionKind Kind { get; private set; }
    public int ScheduledDayOfMonth { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    private RecurringTransaction()
    {
    }

    public RecurringTransaction(
        Guid id,
        Guid userId,
        string description,
        decimal amount,
        TransactionType type,
        RecurringTransactionKind kind,
        DateOnly startDate,
        DateOnly? endDate,
        int scheduledDayOfMonth)
    {
        if (id == Guid.Empty) throw new ArgumentException("Transaction id cannot be empty.", nameof(id));
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        if (scheduledDayOfMonth is < 1 or > 31) throw new ArgumentOutOfRangeException(nameof(scheduledDayOfMonth), "Day of month must be between 1 and 31.");
        if (endDate.HasValue && endDate.Value < startDate)
            throw new ArgumentException("End date cannot be before start date.", nameof(endDate));

        if (kind == RecurringTransactionKind.Salary && type != TransactionType.Income)
            throw new ArgumentException("Salary must be income.");

        if (kind == RecurringTransactionKind.Expense && type != TransactionType.Expense)
            throw new ArgumentException("Expense must be an expense.");

        Id = id;
        UserId = userId;
        Description = description.Trim();
        Amount = amount;
        Type = type;
        Kind = kind;
        ScheduledDayOfMonth = scheduledDayOfMonth;
        StartDate = startDate;
        EndDate = endDate;
    }
    
    public void EndOn(DateOnly endDate)
    {
        if (EndDate.HasValue)
            throw new InvalidOperationException("Recurring transaction has already ended.");

        if (endDate < StartDate)
            throw new InvalidOperationException("Recurring transaction cannot end before it starts.");

        EndDate = endDate;
    }
}