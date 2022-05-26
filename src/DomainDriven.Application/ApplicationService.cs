using System.Reflection;
using DomainDriven.Application.Commands;
using DomainDrivenBase.Domain;
using Microsoft.Extensions.Logging;

namespace DomainDriven.Application
{
    public abstract class ApplicationService : IApplicationService
    {
        private readonly IJournalRepository _journalRepository;
        private readonly ILogger<ApplicationService> _logger;

        protected IJournalRepository JournalRepository => _journalRepository;

        protected ApplicationService(
            IJournalRepository journalRepository,
            ILogger<ApplicationService> logger)
        {
            _journalRepository = journalRepository;
            _logger = logger;
        }

        public async Task<CommandResult> HandleCommand(ICommand command)
        {
            CommandResult? result = null;
            try
            {
                var commandHandleMethods = GetType()
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(x => x.Name.Equals("Handle"))
                    .ToList();

                if (!commandHandleMethods.Any())
                {
                    throw new InvalidOperationException("No 'Handle' methods found in application " +
                                                        $"service '{GetType().AssemblyQualifiedName}'");
                }

                var invoked = false;
                foreach (var method in commandHandleMethods)
                {
                    var parameters = method.GetParameters();
                    if (parameters.All(p => p.ParameterType != command.GetType()))
                        continue;

                    var task = (Task<CommandResult>?)method.Invoke(this, new object[] { command });
                    if (task == null)
                    {
                        throw new InvalidOperationException($"Invoked '{method.Name}' with command of type " +
                                                            $"'{command.GetType().AssemblyQualifiedName}' but " +
                                                            "method returned null. Unable to await task.");
                    }

                    result = await task;

                    invoked = true;
                    break;
                }

                if (!invoked)
                {
                    throw new InvalidOperationException("No 'Handle' method found for command " +
                                                        $"'{command.GetType().AssemblyQualifiedName}' " +
                                                        $"in application service '{GetType().AssemblyQualifiedName}'");
                }

                if (result == null)
                {
                    throw new InvalidOperationException("Unknown error occured while handling command " +
                                                        $"'{command.GetType().AssemblyQualifiedName}' " +
                                                        $"in application service '{GetType().AssemblyQualifiedName}'." +
                                                        "Command result was null.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled exception occurred");
                return CommandResult.Failure(e.Message);
            }
            return result;
        }
    }
}