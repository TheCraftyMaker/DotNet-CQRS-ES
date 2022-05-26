namespace DomainDriven.Infrastructure.Database
{
    public interface IDatabaseSeeder<in TContext>
    {
        void Seed(IDatabase<TContext> database, string tableName);
    }
}