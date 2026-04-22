using FinanceTracker.Application.Interfaces;

namespace FinanceTracker.Application.Commands.DeleteOneOffTransaction;

public class Handler
{
    private readonly IUserRepository _repository;

    public Handler(IUserRepository repository)
    {
        _repository = repository;
    }

    public void Handle(Command command)
    {
        var user = _repository.GetById(command.UserId);
        
        if(user is null)
            throw new InvalidOperationException("User was not found");
        
        user.DeleteOneOffTransaction(command.OneOffTransactionId);
        
        _repository.Update(user);
    }
}