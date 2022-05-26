using DomainDriven.Infrastructure.Bus;
using DomainDriven.Infrastructure.Serialization;

namespace DomainDriven.Infrastructure.Journal
{
      public abstract class SourcedRepository<TEvent>
    {
        private readonly IJournal _journal;
        private readonly ISerializer _serializer;
        private readonly IBus _bus;

        protected EntryStreamReader Reader { get; }

        protected SourcedRepository(IJournal journal, ISerializer serializer, IBus bus)
        {
            _journal = journal;
            _serializer = serializer;
            _bus = bus;

            Reader = journal.StreamReader();
        }

        protected EntryBatch ToBatch(IEnumerable<TEvent> events)
        {
            var eventList = events.ToList();

            var batch = new EntryBatch(eventList.Count);
            foreach (var @event in eventList)
            {
                if(@event == null)
                    continue;
                
                if(string.IsNullOrEmpty(@event.GetType().AssemblyQualifiedName))
                    continue;
                
                var eventBody = _serializer.Serialize(@event);
                batch.AddEntry(@event.GetType().AssemblyQualifiedName!, eventBody);
            }
            return batch;
        }

        protected List<TEvent> ToSourceStream(List<EntryValue> stream)
        {
            var sourceStream = new List<TEvent>(stream.Count);
            foreach (var value in stream)
            {
                var sourceType = Type.GetType(value.Type);
                if (sourceType == null)
                {
                    string typeName;
                    var fullName = value.Type.Split(',')[0];
                    if (value.Type.Contains('+', StringComparison.OrdinalIgnoreCase))
                    {
                        var splinted = fullName.Split('+');
                        typeName = splinted[^1];
                    }
                    else
                    {
                        var splinted = fullName.Split('.');
                        typeName = splinted[^1];
                    }

                    sourceType = AppDomain.CurrentDomain
                        .GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .SingleOrDefault(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
                }

                if (sourceType == null)
                {
                    throw new InvalidOperationException("Unable to convert to source stream. " +
                                                        $"Cannot find type: '{value.Type}'.");
                }

                var source = (TEvent)_serializer.Deserialize(value.Body, sourceType);
                sourceStream.Add(source);
            }

            return sourceStream;
        }

        public async Task Save(string streamName, string streamType,
            int nextVersion, IEnumerable<TEvent> events)
        {
            await _journal.Write(streamName, nextVersion, ToBatch(events));

            await _bus.Publish(new InternalMessage(streamName, streamType, nextVersion));
        }
    }
}