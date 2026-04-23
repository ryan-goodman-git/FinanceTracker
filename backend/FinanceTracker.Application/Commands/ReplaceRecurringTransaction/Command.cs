namespace FinanceTracker.Application.Commands.ReplaceRecurringTransaction;

public sealed record Command(
    Guid UserId,
    Guid RecurringTransactionId,
    string Description,
    decimal Amount,
    int ScheduledDayOfMonth,
    DateOnly ReplacementStartDate);