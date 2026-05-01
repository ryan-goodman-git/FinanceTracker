using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Queries.GetOneOffTransactionById;

public sealed record Result(
    Guid OneOffTransactionId,
    Guid UserId,
    string Description,
    decimal Amount,
    TransactionType Type,
    DateOnly Date);