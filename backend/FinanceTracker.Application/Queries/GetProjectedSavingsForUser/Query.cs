namespace FinanceTracker.Application.Queries.GetProjectedSavingsForUser;

public sealed record Query(Guid UserId, DateOnly Today);