namespace DomainDriven.Infrastructure.Database
{
    public interface IDatabaseInitializer<in TContext>
    {
        Task Initialize(IDatabase<TContext> database);
    }
}