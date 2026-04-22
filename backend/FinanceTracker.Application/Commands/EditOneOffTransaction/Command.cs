namespace FinanceTracker.Application.Commands.EditOneOffTransaction;

public sealed record Command(
    Guid UserId,
    Guid OneOffTransactionId,
    string Description,
    decimal Amount);