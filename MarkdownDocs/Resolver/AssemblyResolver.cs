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
        private readonly IAssemblyBuilder _assemblyBuilder;

        public AssemblyResolver(IAssemblyBuilder assemblyBuilder)
        {
            _assemblyBuilder = assemblyBuilder;

            _assemblyBuilder.Type(typeof(object));
            _assemblyBuilder.Type(typeof(bool));
            _assemblyBuilder.Type(typeof(double));
            _assemblyBuilder.Type(typeof(int));
            _assemblyBuilder.Type(typeof(string));
        }

        public async Task<IAssemblyMetadata> ResolveAsync(IDocsOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Assembly assembly = Assembly.LoadFrom(options.InputPath);
            string? assemblyName = assembly.GetName().Name;

            IEnumerable<Task> tasks = assembly.ExportedTypes.Select(type => ResolveTypeAsync(type, cancellationToken));
            await Task.WhenAll(tasks).ConfigureAwait(false);

            return _assemblyBuilder.WithName(assemblyName).Build();
        }

        private async Task ResolveTypeAsync(Type type, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            TypeMetadata typeRef = _assemblyBuilder.Type(type);

            var tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    foreach (ConstructorInfo ctor in type.GetConstructors())
                    {
                        _assemblyBuilder.Constructor(typeRef, ctor);
                    }
                }, cancellationToken),

                Task.Run(() =>
                {
                    foreach (FieldInfo field in type.GetFields())
                    {
                        _assemblyBuilder.Field(typeRef, field);
                    }
                }, cancellationToken),

                Task.Run(() =>
                {
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        _assemblyBuilder.Property(typeRef, property);
                    }
                }, cancellationToken),

                Task.Run(() =>
                {
                    foreach (MethodInfo method in type.GetMethods())
                    {
                       _assemblyBuilder.Method(typeRef, method);
                    }
                }, cancellationToken),

                Task.Run(() =>
                {
                    foreach (EventInfo ev in type.GetEvents())
                    {
                        _assemblyBuilder.Event(typeRef, ev);
                    }
                }, cancellationToken)
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
