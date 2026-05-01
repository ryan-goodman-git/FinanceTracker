namespace FinanceTracker.Application.Queries.GetUserById;

public sealed record Result(
    Guid UserId,
    string Name,
    decimal InitialBalance,
    DateOnly StartDate);