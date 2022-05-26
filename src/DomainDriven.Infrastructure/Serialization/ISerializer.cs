namespace DomainDriven.Infrastructure.Serialization
{
    public interface ISerializer
    {
        string Serialize(object? instance);
        object Deserialize(string serialized, Type type);
    }
}