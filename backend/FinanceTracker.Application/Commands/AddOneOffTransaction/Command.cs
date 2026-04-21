using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Commands.AddOneOffTransaction;

public sealed record Command(
    Guid UserId, 
    string Description,
    decimal Amount,
    TransactionType Type,
    DateOnly Date);