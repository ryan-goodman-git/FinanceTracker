using FinanceTracker.Application.Interfaces;

namespace FinanceTracker.Application.Queries.GetUsers;

public sealed class Handler
{
    private readonly IUserRepository _userRepository;

    public Handler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public IReadOnlyCollection<Result> Handle(Query query)
    {
        return _userRepository
            .GetAll()
            .Select(user => new Result(
                user.Id,
                user.Name,
                user.InitialBalance,
                user.StartDate))
            .ToList();
    }
}
