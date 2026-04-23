using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Commands.AddRecurringTransaction;

public class Handler
{
    private readonly IUserRepository _userRepository;

    public Handler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Result Handle(Command command)
    {
        var user = _userRepository.GetById(command.UserId);

        if (user is null)
            throw new InvalidOperationException("User was not found.");

        var recurringTransaction = new RecurringTransaction(
            Guid.NewGuid(),
            user.Id,
            command.Description,
            command.Amount,
            TransactionType.Expense,
            RecurringTransactionKind.Expense,
            command.StartDate,
            null,
            command.ScheduledDayOfMonth);
        
        user.AddRecurringTransaction(recurringTransaction);
        
        _userRepository.Update(user);

        return new Result(recurringTransaction.Id);
    }
}