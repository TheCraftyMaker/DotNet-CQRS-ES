namespace DomainDriven.Infrastructure.Database
{
    public interface ISeeder<out TRecord>
    {
        public string Database { get; }
        
        public string Table { get; }

        public IReadOnlyCollection<TRecord> Records();
    }
}