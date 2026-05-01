using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Api.Contracts.Responses.OneOffTransactions;

public sealed record GetOneOffTransactionResponse(
    Guid OneOffTransactionId,
    Guid UserId,
    string Description,
    decimal Amount,
    TransactionType Type,
    DateOnly Date);