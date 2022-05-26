namespace DomainDriven.Application.Commands
{
    public interface IAuditableCommand
    {
        public string UserName { get; }
        public string CorrelationId { get; }
    }
}