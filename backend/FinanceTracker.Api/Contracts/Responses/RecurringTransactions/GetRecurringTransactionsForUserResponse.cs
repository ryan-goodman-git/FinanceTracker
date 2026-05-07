using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Api.Contracts.Responses.RecurringTransactions;

public sealed record GetRecurringTransactionsForUserResponse(
    Guid RecurringTransactionId,
    Guid UserId,
    string Description,
    decimal Amount,
    TransactionType Type,
    RecurringTransactionKind Kind,
    DateOnly StartDate,
    DateOnly? EndDate,
    int ScheduledDayOfMonth);