using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Queries.GetRecurringTransactionsForUser;

public sealed record Result(
    Guid RecurringTransactionId,
    Guid UserId,
    string Description,
    decimal Amount,
    TransactionType Type,
    RecurringTransactionKind Kind,
    DateOnly StartDate,
    DateOnly? EndDate,
    int ScheduledDayOfMonth);