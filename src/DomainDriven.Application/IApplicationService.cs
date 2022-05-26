using DomainDriven.Application.Commands;

namespace DomainDriven.Application
{
    public interface IApplicationService
    {
        Task<CommandResult> HandleCommand(ICommand command);
    }
}