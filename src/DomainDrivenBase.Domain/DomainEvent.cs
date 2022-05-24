namespace DomainDrivenBase.Domain
{
    public abstract class DomainEvent
    {
        public string Id { get; protected init; }
        public string IssuedFor { get; private set; }
        public long OccuredOn { get; private set; }
        public string CorrelationId { get; private set; }

        protected DomainEvent()
        {
            OccuredOn = DateTime.UtcNow.Ticks;
        }
    }
}