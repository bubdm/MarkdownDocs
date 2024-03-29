﻿using MarkdownDocs.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownDocs.Context
{
    public interface IMethodBaseContext : IDocContext
    {
        AccessModifier AccessModifier { get; set; }
        IParameterContext Parameter(int id);
    }

    public class MethodBaseContext : MemberMetadata, IMethodBaseContext
    {
        private readonly Dictionary<int, IParameterContext> _parameters = new Dictionary<int, IParameterContext>();

        public MethodBaseContext(int id, ITypeContext context) : base(id, context.GetMetadata())
        {
            Context = context;
        }

        public IEnumerable<IParameterMetadata> Parameters => _parameters.Values.Select(p => p.GetMetadata());
        public ITypeContext Context { get; }

        public IParameterContext Parameter(int id)
        {
            if (_parameters.TryGetValue(id, out var parameter))
            {
                return parameter;
            }

            var newParameter = new ParameterContext(id, Context);
            _parameters.Add(id, newParameter);

            return newParameter;
        }
    }
}
