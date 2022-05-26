namespace DomainDriven.Infrastructure.Bus
{
    public interface IBus
    {
        Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class;
    }
}