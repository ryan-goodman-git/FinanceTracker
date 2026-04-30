namespace FinanceTracker.Api.Contracts.Responses.Users;

public sealed record GetBalanceResponse(Guid UserId, DateOnly TargetDate, decimal Balance);