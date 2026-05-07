using FinanceTracker.Application.Interfaces;

namespace FinanceTracker.Application.Queries.GetRecurringTransactionsForUser;

public sealed class Handler
{
    private readonly IUserRepository _userRepository;

    public Handler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public IReadOnlyCollection<Result> Handle(Query query)
    {
        var user = _userRepository.GetById(query.UserId);
        
        if(user is null)
            throw new InvalidOperationException("User was not found.");
        
        return user.RecurringTransactions
            .Select(transaction => new Result(
                transaction.Id,
                transaction.UserId,
                transaction.Description,
                transaction.Amount,
                transaction.Type,
                transaction.Kind,
                transaction.StartDate,
                transaction.EndDate,
                transaction.ScheduledDayOfMonth))
            .ToList();
    }
}