namespace DomainDriven.Infrastructure.Database
{
    public interface IDatabase<out TContext>
    {
        TContext Context { get; }
        string JournalStoreName { get; }
        string ReadStoreName { get; }
    }
}