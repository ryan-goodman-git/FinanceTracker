using FinanceTracker.Application.Interfaces;

namespace FinanceTracker.Application.Queries.GetBalanceForUserOnDate;

public  sealed class Handler
{
    private readonly IUserRepository _userRepository;
    
    public Handler(IUserRepository repository)
    {
        _userRepository = repository;
    }

    public Result Handle(Query query)
    {
        var user = _userRepository.GetById(query.UserId);

        if (user is null)
            throw new InvalidOperationException("User was not found.");

        var balance = user.GetBalanceOn(query.TargetDate);

        return new Result(balance);
    }
}