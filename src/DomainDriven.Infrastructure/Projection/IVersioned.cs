namespace DomainDriven.Infrastructure.Projection
{
    public interface IVersioned
    {
        public int Version { get; set; }
    }
}