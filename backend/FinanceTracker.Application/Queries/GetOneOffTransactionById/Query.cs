namespace FinanceTracker.Application.Queries.GetOneOffTransactionById;

public sealed record Query(Guid UserId, Guid TransactionId);