namespace FinanceTracker.Api.Contracts.Responses.Users;

public sealed record GetUserResponse(
    Guid userId, 
    string Name,
    decimal InitialBalance,
    DateOnly StartDate);