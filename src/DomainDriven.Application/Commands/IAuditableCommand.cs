namespace DomainDriven.Application.Commands
{
    public interface IAuditableCommand : ICommand
    {
        public string UserName { get; }
        public string CorrelationId { get; }
    }
}