using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Api.Contracts.Requests.OneOffTransactions;

public sealed record AddOneOffTransactionRequest(
    string Description,
    decimal Amount,
    TransactionType Type,
    DateOnly Date);