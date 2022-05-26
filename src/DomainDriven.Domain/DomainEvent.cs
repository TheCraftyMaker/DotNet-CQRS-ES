namespace DomainDriven.Domain
{
    public abstract class DomainEvent
    {
        public string Id { get; protected init; } = string.Empty;
        public string IssuedFor { get; private set; } = string.Empty;
        public long OccuredOn { get; private set; }
        public string CorrelationId { get; private set; } = string.Empty;

        protected DomainEvent()
        {
            OccuredOn = DateTime.UtcNow.Ticks;
        }
    }
}