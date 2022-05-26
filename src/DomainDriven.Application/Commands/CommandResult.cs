namespace DomainDriven.Application.Commands
{
    public class CommandResult
    {
        private readonly List<string> _errorMessages;
        private readonly List<Type> _expectedEvents;

        public IReadOnlyCollection<string> ErrorMessages => _errorMessages;
        public IReadOnlyCollection<string?> ExpectedEvents => _expectedEvents.Select(x => x.FullName).ToList();
        
        public bool Succeeded => !_errorMessages.Any();

        private CommandResult()
        {
            _errorMessages = new List<string>();
            _expectedEvents = new List<Type>();
        }

        private CommandResult(IEnumerable<Type> expectedEvents)
            : this()
        {
            _errorMessages = new List<string>();

            _expectedEvents.Clear();
            _expectedEvents.AddRange(expectedEvents);
        }

        public static CommandResult Success()
        {
            return new();
        }

        public static CommandResult Success(params Type[] expectedEvents)
        {
            return new(expectedEvents);
        }

        public static CommandResult Failure(string message)
        {
            var result = new CommandResult();
            result.AddMessage(message);

            return result;
        }

        public static CommandResult Failure(IEnumerable<string> messages)
        {
            var result = new CommandResult();
            result.AddMessages(messages);

            return result;
        }

        private void AddMessage(string message)
        {
            _errorMessages.Add(message);
        }

        private void AddMessages(IEnumerable<string> messages)
        {
            _errorMessages.AddRange(messages);
        }
    }
}