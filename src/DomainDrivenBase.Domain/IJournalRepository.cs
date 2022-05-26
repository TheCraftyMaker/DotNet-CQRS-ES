namespace DomainDrivenBase.Domain
{
    public interface IJournalRepository
    {
        Task<TEntity?> Load<TEntity, TId>(string id) 
            where TId : Value<TId>
            where TEntity : SourcedEntity<TId, DomainEvent>;

        Task Save<TId>(SourcedEntity<TId, DomainEvent> level)
            where TId : Value<TId>;
    }
}