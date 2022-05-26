namespace DomainDriven.Domain
{
    public interface IJournalRepository
    {
        Task<TEntity?> Load<TEntity, TId>(TId id, Func<TId, string> idValueExtractor) 
            where TId : Value<TId>
            where TEntity : SourcedEntity<TId, DomainEvent>;

        Task Save<TId>(SourcedEntity<TId, DomainEvent> level)
            where TId : Value<TId>;
    }
}