using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Api.Contracts.Responses.RecurringTransactions;

public sealed record GetRecurringTransactionResponse(
    Guid RecurringTransactionId,
    Guid UserId,
    string Description,
    decimal Amount,
    TransactionType Type,
    RecurringTransactionKind Kind,
    int ScheduledDayOfMonth,
    DateOnly StartDate,
    DateOnly? EndDate);