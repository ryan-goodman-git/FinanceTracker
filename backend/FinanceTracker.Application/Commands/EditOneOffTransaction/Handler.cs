using FinanceTracker.Application.Interfaces;

namespace FinanceTracker.Application.Commands.EditOneOffTransaction;

public class Handler
{
    private readonly IUserRepository _userRepository;
    
    public Handler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void Handle(Command command)
    {
        var user = _userRepository.GetById(command.UserId);

        if (user is null)
            throw new InvalidOperationException("User was not found.");
        
        user.EditOneOffTransaction(command.OneOffTransactionId, command.Description, command.Amount);
        
        _userRepository.Update(user);
    } 
}