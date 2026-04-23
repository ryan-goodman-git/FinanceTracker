using FinanceTracker.Application.Interfaces;

namespace FinanceTracker.Application.Commands.ReplaceRecurringTransaction;

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
        
        var replacement = user.ReplaceRecurringTransaction(
            command.RecurringTransactionId,
            command.Description,
            command.Amount,
            command.ScheduledDayOfMonth,
            command.ReplacementStartDate);
        
        _userRepository.Update(user);

        return new Result(replacement.Id);
    }
}