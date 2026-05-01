using FinanceTracker.Application.Interfaces;

namespace FinanceTracker.Application.Queries.GetUserById;

public sealed class Handler
{
    private readonly IUserRepository _userRepository;
    
    public Handler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Result Handle(Query query)
    {
        var user = _userRepository.GetById(query.UserId);

        if (user is null)
            throw new InvalidOperationException("User was not found.");
        
        return new Result(
            user.Id,
            user.Name,
            user.InitialBalance,
            user.StartDate);
    }
}