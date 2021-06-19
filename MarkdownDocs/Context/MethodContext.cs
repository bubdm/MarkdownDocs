using MarkdownDocs.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownDocs.Context
{
    public class MethodContext : MemberMetadata, IMethodMetadata, IMethodContext
    {
        private readonly Dictionary<int, IParameterContext> _parameters = new Dictionary<int, IParameterContext>();
        private readonly ITypeContext _ownerContext;

        public MethodContext(int id, ITypeContext owner) : base(id, owner.GetMetadata())
        {
            _ownerContext = owner;
        }

        public IParameterContext Parameter(int id)
        {
            if (_parameters.TryGetValue(id, out var parameter))
            {
                return parameter;
            }

            var newParameter = new ParameterContext(_ownerContext, id);
            _parameters.Add(id, newParameter);

            return newParameter;
        }

        public void Return(ITypeContext type)
        {
            ReturnType = type.GetMetadata();
            type.Reference(_ownerContext);
        }

        public IMethodMetadata GetMetadata() => this;

        public IEnumerable<IParameterMetadata> Parameters => _parameters.Values.Select(p => p.GetMetadata());
        public ITypeMetadata ReturnType { get; private set; } = default!;
    }
}