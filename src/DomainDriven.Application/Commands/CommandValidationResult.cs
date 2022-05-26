namespace DomainDriven.Application.Commands
{
    public class CommandValidationResult<TEntity>
    {
        private readonly List<string> _messages;

        public TEntity? Entity { get; private init; }
        public IReadOnlyCollection<string> Messages => _messages;
        public bool IsValid => !_messages.Any();

        private CommandValidationResult()
        {
            _messages = new List<string>();
        }

        public static CommandValidationResult<TEntity> Valid()
        {
            return new();
        }

        public static CommandValidationResult<TEntity> Valid(TEntity entity)
        {
            return new()
            {
                Entity = entity
            };
        }

        public static CommandValidationResult<TEntity> Invalid(string message)
        {
            var result = new CommandValidationResult<TEntity>();
            result.AddMessage(message);

            return result;
        }

        public static CommandValidationResult<TEntity> Invalid(IEnumerable<string> messages)
        {
            var result = new CommandValidationResult<TEntity>();
            result.AddMessages(messages);

            return result;
        }

        public CommandValidationResult<TEntity> Merge(params CommandValidationResult<TEntity>[] results)
        {
            foreach (var result in results)
            {
                if(!result.Messages.Any())
                    continue;

                _messages.AddRange(result._messages);
            }
            return this;
        }

        private void AddMessage(string message)
        {
            _messages.Add(message);
        }

        private void AddMessages(IEnumerable<string> messages)
        {
            _messages.AddRange(messages);
        }
    }
}