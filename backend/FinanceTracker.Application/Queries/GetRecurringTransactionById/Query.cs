namespace FinanceTracker.Application.Queries.GetRecurringTransactionById;

public sealed record Query(Guid UserId, Guid RecurringTransactionId);