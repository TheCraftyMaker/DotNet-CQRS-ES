namespace DomainDriven.Application.Commands
{
    public class AuditableCommand : IAuditableCommand 
    {
        public string UserName { get; } = string.Empty;

        public string CorrelationId { get; } = string.Empty;
    }
}