namespace DomainDrivenBase.Domain
{
    public abstract class SourcedEntity<TId, TEvent>
        where TId : Value<TId>
    {
        private readonly List<TEvent> _applied;
        private readonly TId _id;
        
        public int CurrentVersion { get; private set; }
        public TId Id => _id;
        public IReadOnlyCollection<TEvent> Applied => _applied;
        public int NextVersion => CurrentVersion + 1;

        protected SourcedEntity(TId id, int streamVersion = 0)
        {
            _applied = new List<TEvent>();
            _id = id;
            
            CurrentVersion = streamVersion;
        }
        
        protected SourcedEntity(IEnumerable<TEvent> stream, int streamVersion, TId id)
            : this(id, streamVersion)
        {
            foreach (var @event in stream)
            {
                DispatchWhen(@event);
            }
        }

        protected abstract void EnsureValidState();

        protected void Apply(TEvent @event)
        {
            DispatchWhen(@event);

            EnsureValidState();

            _applied.Add(@event);
        }
        
        protected void DispatchWhen(TEvent @event)
        {
            ((dynamic)this).When(@event as dynamic);
        }

        public void MarkCommitted()
        {
            _applied.Clear();
            CurrentVersion += 1;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override bool Equals(object? other)
        {
            if (other == null || other.GetType() != GetType())
            {
                return false;
            }

            var otherProposal = (SourcedEntity<TId, TEvent>)other;

            return _id.Equals(otherProposal.Id);
        }

        public override string ToString()
        {
            return $"SourcedEntity[id={_id} current version={CurrentVersion} " +
                   $"sourced type={typeof(TEvent).AssemblyQualifiedName}";
        }
    }
}