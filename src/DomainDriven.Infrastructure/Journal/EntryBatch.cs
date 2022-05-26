namespace DomainDriven.Infrastructure.Journal
{
    public class EntryBatch
    {
        public List<Entry> Entries { get; }

        public EntryBatch(int entries)
        {
            Entries = new List<Entry>(entries);
        }

        public EntryBatch()
            : this(2)
        {
        }

        public EntryBatch(string type, string body)
            : this(type, body, string.Empty)
        {
        }

        public EntryBatch(string type, string body, string snapshot)
            : this()
        {
            AddEntry(type, body, snapshot);
        }

        public void AddEntry(string type, string body)
        {
            Entries.Add(new Entry(type, body));
        }

        public void AddEntry(string type, string body, string snapshot)
        {
            Entries.Add(new Entry(type, body, snapshot));
        }

        public class Entry
        {
            public string Body { get; }
            public string Type { get; }
            public string Snapshot { get; }

            internal Entry(string type, string body)
                : this(type, body, string.Empty)
            {
            }

            internal Entry(string type, string body, string snapshot)
            {
                Type = type;
                Body = body;
                Snapshot = snapshot;
            }
        }
    }
}