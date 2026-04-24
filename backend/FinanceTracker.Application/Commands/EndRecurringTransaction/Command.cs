namespace FinanceTracker.Application.Commands.EndRecurringTransaction;

public sealed record Command(Guid UserId, Guid RecurringTransactionId, DateOnly EndDate);