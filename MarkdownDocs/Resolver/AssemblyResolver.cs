using MarkdownDocs.Context;
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
        private readonly Func<ITypeContext, ITypeResolver, IMethodResolver> _methodResolverFactory;
        private readonly Func<ITypeContext, ITypeResolver, IConstructorResolver> _constructorResolverFactory;
        private readonly Func<ITypeContext, ITypeResolver, IFieldResolver> _fieldResolverFactory;
        private readonly Func<ITypeContext, ITypeResolver, IPropertyResolver> _propertyResolverFactory;

        public AssemblyResolver(IAssemblyContext assemblyBuilder,
            Func<IAssemblyContext, ITypeResolver> typeResolver,
            Func<ITypeContext, ITypeResolver, IMethodResolver> methodResolverFactory,
            Func<ITypeContext, ITypeResolver, IConstructorResolver> constructorResolverFactory,
            Func<ITypeContext, ITypeResolver, IFieldResolver> fieldResolverFactory,
            Func<ITypeContext, ITypeResolver, IPropertyResolver> propertyResolverFactory)
        {
            _assemblyBuilder = assemblyBuilder;
            _methodResolverFactory = methodResolverFactory;
            _constructorResolverFactory = constructorResolverFactory;
            _fieldResolverFactory = fieldResolverFactory;
            _propertyResolverFactory = propertyResolverFactory;
            _typeResolver = typeResolver(assemblyBuilder);
        }

        public async Task<IAssemblyContext> ResolveAsync(IDocsOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Assembly assembly = Assembly.LoadFrom(options.InputPath);
            string? assemblyName = assembly.GetName().Name;

            IEnumerable<Task> tasks = assembly.ExportedTypes.Select(type => ResolveTypeAsync(type, cancellationToken));
            await Task.WhenAll(tasks).ConfigureAwait(false);

            return _assemblyBuilder.WithName(assemblyName);
        }

        private async Task ResolveTypeAsync(Type type, CancellationToken cancellationToken)
        {
            ITypeContext context = _typeResolver.Resolve(type);
            BindingFlags searchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            var tasks = new List<Task>
            {
                // Resolve constructors
                Task.Run(() =>
                {
                    IConstructorResolver constructorResolver = _constructorResolverFactory(context, _typeResolver);
                    foreach (ConstructorInfo ctor in type.GetConstructors(searchFlags).Where(m => (m.IsPublic || m.IsFamily) && !m.DeclaringType!.IsSubclassOf(typeof(Delegate)) && m.GetParameters().Length > 0))
                    {
                        constructorResolver.Resolve(ctor);
                    }
                }, cancellationToken),

                // Resolve fields
                Task.Run(() =>
                {
                    IFieldResolver fieldResolver = _fieldResolverFactory(context, _typeResolver);
                    foreach (FieldInfo field in type.GetFields(searchFlags).Where(m => (m.IsPublic || m.IsFamily) && !m.IsSpecialName))
                    {
                        fieldResolver.Resolve(field);
                    }
                }, cancellationToken),

                // Resolve properties
                Task.Run(() =>
                {
                    IPropertyResolver propertyResolver = _propertyResolverFactory(context, _typeResolver);
                    foreach (PropertyInfo property in type.GetProperties(searchFlags).Where(m =>
                    (m.CanRead || m.CanWrite)
                    && !m.IsSpecialName
                    && ((m.GetMethod?.IsPublic ?? false)
                        || (m.GetMethod?.IsFamily ?? false)
                        || (m.SetMethod?.IsPublic ?? false)
                        || (m.SetMethod?.IsFamily ?? false))
                    ))
                    {
                        propertyResolver.Resolve(property);
                    }
                }, cancellationToken),

                // Resolve methods
                Task.Run(() =>
                {
                    if(!type.IsSubclassOf(typeof(Delegate)))
                    {
                        IMethodResolver methodResolver = _methodResolverFactory(context, _typeResolver);
                        foreach (MethodInfo method in type.GetMethods(searchFlags).Where(m => (m.IsPublic || m.IsFamily) && !m.IsSpecialName))
                        {
                            methodResolver.Resolve(method);
                        }
                    }
                }, cancellationToken),

                // Resolve events
                Task.Run(() =>
                {
                    foreach (EventInfo ev in type.GetEvents(searchFlags).Where(m => ((m.AddMethod?.IsPublic ?? false) || (m.AddMethod?.IsFamily ?? false)) && !m.IsSpecialName))
                    {
                        //_assemblyBuilder.Event(typeRef, ev);
                    }
                }, cancellationToken)
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
