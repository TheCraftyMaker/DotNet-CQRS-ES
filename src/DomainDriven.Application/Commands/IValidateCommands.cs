namespace DomainDriven.Application.Commands
{
    public interface IValidateCommands<TEntity>
    {
        Task<CommandValidationResult<TEntity?>> Validate(ICommand command);
    }
}