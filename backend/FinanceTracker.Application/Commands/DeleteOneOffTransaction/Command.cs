namespace FinanceTracker.Application.Commands.DeleteOneOffTransaction;

public sealed record Command(Guid UserId, Guid OneOffTransactionId);