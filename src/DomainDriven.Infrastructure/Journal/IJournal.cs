namespace DomainDriven.Infrastructure.Journal
{
    public interface IJournal
    {
        EntryStreamReader StreamReader();
        Task Write(string streamName, int streamVersion, EntryBatch batch);
    }
}