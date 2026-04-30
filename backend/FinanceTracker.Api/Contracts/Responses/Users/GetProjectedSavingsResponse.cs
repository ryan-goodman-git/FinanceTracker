namespace FinanceTracker.Api.Contracts.Responses.Users;

public sealed record GetProjectedSavingsResponse(
        Guid UserId,
        DateOnly Today,
        DateOnly CycleEndDate,
        decimal ProjectedSavings
);