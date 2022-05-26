namespace DomainDriven.Infrastructure.Bus
{
    public sealed class InternalMessage
    {
        public bool Handled { get; private set; }
        public string StreamName { get; }
        public int Version { get; }
        public string StreamType { get; }

        public InternalMessage(string streamName, string streamType, int version)
        {
            StreamName = streamName;
            StreamType = streamType;
            Version = version;
            Handled = false;
        }

        public void SetHandled()
        {
            Handled = true;
        }
    }
}