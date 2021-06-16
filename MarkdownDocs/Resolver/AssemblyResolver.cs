using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Resolver
{
    public class AssemblyResolver : IAssemblyResolver
    {
        private readonly IAssemblyContext _assemblyBuilder;
        private readonly ITypeResolver _typeResolver;
        private readonly Func<ITypeResolver, ITypeContext, IMethodResolver> _methodResolverFactory;

        public AssemblyResolver(IAssemblyContext assemblyBuilder,
            Func<IAssemblyContext, ITypeResolver> typeResolver,
            Func<ITypeResolver, ITypeContext, IMethodResolver> methodResolverFactory)
        {
            _assemblyBuilder = assemblyBuilder;
            _methodResolverFactory = methodResolverFactory;
            _typeResolver = typeResolver(assemblyBuilder);
        }

        public async Task<IAssemblyMetadata> ResolveAsync(IDocsOptions options, CancellationToken cancellationToken)
        {
            _typeResolver.Resolve(typeof(object));
            _typeResolver.Resolve(typeof(bool));
            _typeResolver.Resolve(typeof(double));
            _typeResolver.Resolve(typeof(int));
            _typeResolver.Resolve(typeof(string));

            cancellationToken.ThrowIfCancellationRequested();
            Assembly assembly = Assembly.LoadFrom(options.InputPath);
            string? assemblyName = assembly.GetName().Name;

            IEnumerable<Task> tasks = assembly.ExportedTypes.Select(type => ResolveTypeAsync(type, cancellationToken));
            await Task.WhenAll(tasks).ConfigureAwait(false);

            return _assemblyBuilder.WithName(assemblyName).Build();
        }

        private async Task ResolveTypeAsync(Type type, CancellationToken cancellationToken)
        {
            ITypeContext context = _typeResolver.Resolve(type);

            var tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    foreach (ConstructorInfo ctor in type.GetConstructors())
                    {
                        //_typeResolver.Resolve(type, ctor);
                    }
                }, cancellationToken),

                Task.Run(() =>
                {
                    foreach (FieldInfo field in type.GetFields())
                    {
                        //_assemblyBuilder.Field(typeRef, field);
                    }
                }, cancellationToken),

                Task.Run(() =>
                {
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        //_assemblyBuilder.Property(typeRef, property);
                    }
                }, cancellationToken),

                Task.Run(() =>
                {
                    IMethodResolver methodResolver = _methodResolverFactory(_typeResolver, context);
                    foreach (MethodInfo method in type.GetMethods().Where(m => !m.IsSpecialName && m.DeclaringType == type))
                    {
                        methodResolver.Resolve(method);
                    }
                }, cancellationToken),

                Task.Run(() =>
                {
                    foreach (EventInfo ev in type.GetEvents())
                    {
                        //_assemblyBuilder.Event(typeRef, ev);
                    }
                }, cancellationToken)
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
