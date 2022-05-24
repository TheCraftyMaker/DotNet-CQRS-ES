namespace DomainDrivenBase.Domain
{
    public static class DomainExceptions
    {
        public class InvalidEntityStateException : Exception
        {
            public InvalidEntityStateException(object entity, string streamName, string message)
                : base($"Entity {streamName}-{entity.GetType().Name} state change rejected, {message}")
            {
            }
        }
    }
}