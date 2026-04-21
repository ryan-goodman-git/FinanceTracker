using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Commands.AddOneOffTransaction;

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

        var oneOffTransaction = new OneOffTransaction(
            Guid.NewGuid(),
            user.Id,
            command.Description,
            command.Amount,
            command.Type,
            command.Date);
        
        user.AddOneOffTransaction(oneOffTransaction);
        
        _userRepository.Update(user);

        return new Result(oneOffTransaction.Id);
    }
}