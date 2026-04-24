using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Commands.CreateUser;

public class Handler
{
    private readonly IUserRepository _userRepository;
    
    public Handler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Result Handle(Command command)
    {
        var userId = Guid.NewGuid();
        
        var user = User.Create(
            userId,
            command.Name,
            command.InitialBalance,
            command.StartDate,
            command.SalaryAmount,
            command.SalaryDayOfMonth);
        
        _userRepository.Add(user);
        
        return new Result(userId);
    }
}