using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Queries.GetRecurringTransactionById;

public sealed record Result(
    Guid RecurringTransactionId,
    Guid UserId,
    string Description,
    decimal Amount,
    TransactionType Type,
    RecurringTransactionKind Kind,
    int ScheduledDayOfMonth,
    DateOnly StartDate,
    DateOnly? EndDate);