using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public interface IMethodMetadata : IMemberMetadata
    {
        public IEnumerable<IParameterMetadata> Parameters { get; }
        public ITypeMetadata ReturnType { get; set; }
    }

    public interface IMethodContext : IMethodMetadata
    {
        IParameterMetadata Parameter(int id);
    }

    public class MethodContext : MemberMetadata, IMethodContext
    {
        private readonly Dictionary<int, ParameterMetadata> _parameters = new Dictionary<int, ParameterMetadata>();
        public MethodContext(int id, ITypeMetadata owner) : base(id, owner)
        {
        }

        public IParameterMetadata Parameter(int id)
        {
            if (_parameters.TryGetValue(id, out var parameter))
            {
                return parameter;
            }

            var newParameter = new ParameterMetadata(id);
            _parameters.Add(id, newParameter);

            return newParameter;
        }

        public IEnumerable<IParameterMetadata> Parameters => _parameters.Values;
        public ITypeMetadata ReturnType { get; set; } = default!;
    }
}