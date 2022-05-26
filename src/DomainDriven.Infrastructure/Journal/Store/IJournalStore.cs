namespace DomainDriven.Infrastructure.Journal.Store
{
    public interface IJournalStore
    {
        void Open();
        void Close();
        Task Add(EntryValue entry);
        Task Add(IEnumerable<EntryValue> entries);
        Task<int> LastId();
        Task<EntryValue> GetEntry(int id);
        Task<IEnumerable<EntryValue>> GetStream(string stream);
        Task<int> GetCurrentStreamVersion(string stream);
    }
}