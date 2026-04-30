namespace FinanceTracker.Api.Contracts.Requests.OneOffTransactions;

public sealed record EditOneOffTransactionRequest(string Description, decimal Amount);