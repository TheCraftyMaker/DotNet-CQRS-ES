namespace DomainDriven.Infrastructure.Projection
{
    public interface IProject<TReadModel>
    {
        Task Project(string streamName, int streamVersion);
    }
}