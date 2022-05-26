using DomainDriven.Infrastructure.Journal.Store;

namespace DomainDriven.Infrastructure.Journal
{
    public class Journal : IJournal, IDisposable
    {
        private readonly IJournalStore _store;
        
        public Journal(IJournalStore store)
        {
            _store = store;
            _store.Open();
        }

        public EntryStreamReader StreamReader()
        {
            return new(this);
        }

        public async Task Write(string streamName, int streamVersion, EntryBatch batch)
        {
            var entries = batch.Entries.Select(x =>
                new EntryValue(streamName, streamVersion, x.Type, x.Body, x.Snapshot));

            await _store.Add(entries);
        }

        internal async Task<int> GreatestId()
        {
            return await _store.LastId();
        }

        internal async Task<EntryStream> ReadStream(string streamName)
        {
            EntryValue? latestSnapshotValue = null;

            var values = new List<EntryValue>();
            var storeValues = await _store.GetStream(streamName);
            foreach (var value in storeValues)
            {
                if (value.HasSnapshot())
                {
                    values.Clear();
                    latestSnapshotValue = value;
                }
                else
                {
                    values.Add(value);
                }
            }

            var snapshotVersion = latestSnapshotValue?.StreamVersion ?? 0;
            var streamVersion = values.Count == 0 ? snapshotVersion : values[^1].StreamVersion;

            return new EntryStream(streamName, streamVersion, values,
                latestSnapshotValue == null ? string.Empty : latestSnapshotValue.Snapshot);
        }

        public void Dispose() => _store.Close();
    }
}