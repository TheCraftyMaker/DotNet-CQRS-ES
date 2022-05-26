using System.Reflection;
using System.Text;
using DomainDriven.Infrastructure.Journal;
using DomainDriven.Infrastructure.Serialization;
using Microsoft.Extensions.Logging;

namespace DomainDriven.Infrastructure.Projection
{
    public abstract class Projector<TReadModel, TEvent> : IProject<TReadModel>
        where TReadModel : class, IVersioned, new()
    {
        private readonly IJournal _journal;
        private readonly ISerializer _serializer;
        private readonly IProjectionRepository<TReadModel> _projectionRepository;
        private readonly Dictionary<Type, MethodInfo> _projectMethods;

        protected Projector(
            IJournal journal,
            ISerializer serializer,
            IProjectionRepository<TReadModel> projectionRepository)
        {
            _journal = journal;
            _serializer = serializer;
            _projectionRepository = projectionRepository;

            _projectMethods = GetProjectMethods();
        }

        public async Task Project(string streamName, int streamVersion)
        {
            //Get current model (eventual consistent)
            var model = await _projectionRepository.Find(streamName);
            var isNewReadModel = model == null;

            //Get stream of events for this model
            var reader = _journal.StreamReader();
            var stream = await reader.StreamFor(streamName);
            
            int projectedToVersion;
            //Model does not exist. Create using full stream.
            if (model == null)
            {
                projectedToVersion = InternalProject(ref model, stream.Stream);
            }
            //Model verion is higher than stream version. Rebuild using full stream
            //** edge case **
            else if (model.Version > streamVersion)
            {
                model = new TReadModel();
                projectedToVersion = InternalProject(ref model, stream.Stream);
            }
            //Stale model. Catch up using part of stream
            else if (model.Version < streamVersion)
            {
                var subStream = stream.Stream
                    .Where(e => e.StreamVersion > model.Version)
                    .ToList();

                projectedToVersion = InternalProject(ref model, subStream);
            }
            else //Do nothing
            {
                return;
            }

            if (model == null)
            {
                throw new InvalidOperationException($"Readmodel {typeof(TReadModel).AssemblyQualifiedName} " +
                                                    $"was null after projection. Projection failed for stream {streamName}");
            }

            model.Version = projectedToVersion;
            if (isNewReadModel)
            {
                await _projectionRepository.InsertProjection(model);
            }
            else
            {
                await _projectionRepository.UpdateProjection(model);
            }
        }

        private int InternalProject(ref TReadModel? model, IEnumerable<EntryValue> stream)
        {
            var projectedToVersion = 0;
            foreach (var value in stream.OrderBy(x => x.StreamVersion))
            {
                var sourceType = Type.GetType(value.Type, false);
                if (sourceType == null)
                {
                    throw new InvalidOperationException($"Unable to project event '{value.Type}' for model " +
                                                        $"'{typeof(TReadModel).AssemblyQualifiedName}' with " +
                                                        $"stream name '{value.StreamName}'. Unable to find " +
                                                        $"type {value.Type}. Projection failed.");
                }
                
                
                var source = (TEvent) _serializer.Deserialize(value.Body, sourceType);
                if (source == null)
                {
                    throw new InvalidOperationException($"Unable to project event '{value.Type}' for model " +
                                                        $"'{typeof(TReadModel).AssemblyQualifiedName}' with " +
                                                        $"stream name '{value.StreamName}'. Unable to deserialize " +
                                                        $"{value.Body} into type {sourceType}. Projection failed.");
                }

                if (!_projectMethods.TryGetValue(source.GetType(), out var method))
                {
                    throw new InvalidOperationException($"Unable to project event '{source.GetType().AssemblyQualifiedName}' " +
                                                        $"for model '{typeof(TReadModel).AssemblyQualifiedName}' with " +
                                                        $"stream name '{value.StreamName}'. No project method found in " +
                                                        $"projector '{ GetType().AssemblyQualifiedName}'. Projection failed.");
                }

                var result = (Projection?) method.Invoke(this, new object?[] {model, source});
                if (result == null)
                {
                    throw new InvalidOperationException($"Invoked '{method.Name}' with event of type " +
                                                        $"'{source.GetType().AssemblyQualifiedName}' in " +
                                                        $"projector '{GetType().AssemblyQualifiedName}' but " +
                                                        "method returned null. Projection failed.");
                }

                if (!result.Success)
                {
                    var sb = new StringBuilder();
                    foreach (var message in result.ErrorMessages)
                    {
                        sb.AppendLine($"{message}|");
                    }

                    throw new InvalidOperationException($"Invoked '{method.Name}' with event of type " +
                                                        $"'{source.GetType().AssemblyQualifiedName}' in " +
                                                        $"projector '{GetType().AssemblyQualifiedName}' but " +
                                                        $"method returned the following failures: {sb}. " +
                                                        "Projection failed.");
                }

                model = result.Model;
                projectedToVersion = value.StreamVersion;
            }

            return projectedToVersion;
        }

        private Dictionary<Type, MethodInfo> GetProjectMethods()
        {
            var methods = new Dictionary<Type, MethodInfo>();

            var projectMethods = GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.Name.Equals("Project", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!projectMethods.Any())
            {
                throw new InvalidOperationException("No 'Project' methods found in " +
                                                    $"projector '{GetType().AssemblyQualifiedName}'");
            }

            foreach (var method in projectMethods)
            {
                var parameter = method
                    .GetParameters()
                    .Where(p => p.ParameterType.IsAssignableTo(typeof(TEvent)))
                    .Select(x => x.ParameterType)
                    .FirstOrDefault();

                if (parameter == null)
                {
                    throw new InvalidOperationException("Found project method with no parameters " +
                                                        $"in projector '{GetType().AssemblyQualifiedName}'. " +
                                                        "Project method should have 1 parameter of type " +
                                                        $"'{typeof(TEvent).AssemblyQualifiedName}'.");
                }

                methods.Add(parameter, method);
            }

            return methods;
        }

        public class Projection
        {
            private readonly List<string> _errorMessages;
            public bool Success => _errorMessages.Count == 0;
            public IReadOnlyCollection<string> ErrorMessages => _errorMessages;
            public TReadModel? Model { get; }

            private Projection()
            {
                _errorMessages = new List<string>();
            }

            private Projection(TReadModel model)
                : this()
            {
                Model = model;
            }

            public static Projection Completed(TReadModel model)
            {
                return new(model);
            }

            public static Projection Error(string message)
            {
                return new Projection().AddErrorMessage(message);
            }

            public Projection AddErrorMessage(string error)
            {
                _errorMessages.Add(error);
                return this;
            }
        }
    }
}