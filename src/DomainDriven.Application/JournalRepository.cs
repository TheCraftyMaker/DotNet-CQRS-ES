using DomainDriven.Domain;
using DomainDriven.Infrastructure.Bus;
using DomainDriven.Infrastructure.Journal;
using DomainDriven.Infrastructure.Serialization;

namespace DomainDriven.Application
{
    internal class JournalRepository : SourcedRepository<DomainEvent>, IJournalRepository
    {
        public JournalRepository(IJournal journal, ISerializer serializer, IBus bus)
            : base(journal, serializer, bus)
        {
        }

        public async Task<TEntity?> Load<TEntity, TId>(TId id, Func<TId, string> idValueExtractor)
            where TId : Value<TId>
            where TEntity : SourcedEntity<TId, DomainEvent>
        {
            var idValue = idValueExtractor(id);
            
            var stream = await Reader.StreamFor(idValue);

            if (!stream.Stream.Any())
                return null;

            return (TEntity?) Activator.CreateInstance(typeof(TEntity),
                ToSourceStream(stream.Stream), stream.StreamVersion, id);
        }

        public async Task Save<TId>(SourcedEntity<TId, DomainEvent> entity) 
            where TId : Value<TId>
        {
            if (!entity.Applied.Any())
                return;

            await Save(entity.Id.ToString(), entity.GetType().AssemblyQualifiedName!,
                entity.NextVersion, entity.Applied);

            entity.MarkCommitted();
        }
    }
}