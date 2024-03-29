﻿using MarkdownDocs.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Resolver
{
    public class AssemblyResolver : IDocResolver
    {
        private readonly IDocsOptions _options;
        private readonly Func<IAssemblyContext, ITypeResolver> _typeResolverFactory;
        private readonly Func<ITypeContext, ITypeResolver, IMethodResolver> _methodResolverFactory;
        private readonly Func<ITypeContext, ITypeResolver, IConstructorResolver> _constructorResolverFactory;
        private readonly Func<ITypeContext, ITypeResolver, IFieldResolver> _fieldResolverFactory;
        private readonly Func<ITypeContext, ITypeResolver, IPropertyResolver> _propertyResolverFactory;
        private readonly Func<ITypeContext, ITypeResolver, IEventResolver> _eventResolverFactory;

        public AssemblyResolver(IDocsOptions options,
            Func<IAssemblyContext, ITypeResolver> typeResolverFactory,
            Func<ITypeContext, ITypeResolver, IMethodResolver> methodResolverFactory,
            Func<ITypeContext, ITypeResolver, IConstructorResolver> constructorResolverFactory,
            Func<ITypeContext, ITypeResolver, IFieldResolver> fieldResolverFactory,
            Func<ITypeContext, ITypeResolver, IPropertyResolver> propertyResolverFactory,
            Func<ITypeContext, ITypeResolver, IEventResolver> eventResolverFactory)
        {
            _options = options;
            _typeResolverFactory = typeResolverFactory;
            _methodResolverFactory = methodResolverFactory;
            _constructorResolverFactory = constructorResolverFactory;
            _fieldResolverFactory = fieldResolverFactory;
            _propertyResolverFactory = propertyResolverFactory;
            _eventResolverFactory = eventResolverFactory;
        }

        public async Task ResolveAsync(IAssemblyContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ITypeResolver typeResolver = _typeResolverFactory(context);

            Assembly assembly = Assembly.LoadFrom(_options.InputPath);
            string? assemblyName = assembly.GetName().Name;
            context.Name = assemblyName;

            IEnumerable<Task> tasks = assembly.ExportedTypes.Select(type => ResolveTypeAsync(typeResolver, type, cancellationToken));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task ResolveTypeAsync(ITypeResolver typeResolver, Type type, CancellationToken cancellationToken)
        {
            ITypeContext context = typeResolver.Resolve(type);
            BindingFlags searchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            var tasks = new List<Task>
            {
                // Resolve constructors
                Task.Run(() =>
                {
                    IConstructorResolver constructorResolver = _constructorResolverFactory(context, typeResolver);
                    foreach (ConstructorInfo ctor in type.GetConstructors(searchFlags).Where(m => (m.IsPublic || m.IsFamily) && !m.DeclaringType!.IsSubclassOf(typeof(Delegate))))
                    {
                        constructorResolver.Resolve(ctor);
                    }
                }, cancellationToken),

                // Resolve fields
                Task.Run(() =>
                {
                    IFieldResolver fieldResolver = _fieldResolverFactory(context, typeResolver);
                    foreach (FieldInfo field in type.GetFields(searchFlags).Where(m => (m.IsPublic || m.IsFamily) && !m.IsSpecialName))
                    {
                        fieldResolver.Resolve(field);
                    }
                }, cancellationToken),

                // Resolve properties
                Task.Run(() =>
                {
                    IPropertyResolver propertyResolver = _propertyResolverFactory(context, typeResolver);
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
                        IMethodResolver methodResolver = _methodResolverFactory(context, typeResolver);
                        foreach (MethodInfo method in type.GetMethods(searchFlags).Where(m => (m.IsPublic || m.IsFamily) && !m.IsSpecialName))
                        {
                            methodResolver.Resolve(method);
                        }
                    }
                }, cancellationToken),

                // Resolve events
                Task.Run(() =>
                {
                    IEventResolver eventResolver = _eventResolverFactory(context, typeResolver);
                    foreach (EventInfo ev in type.GetEvents(searchFlags).Where(m => ((m.AddMethod?.IsPublic ?? false) || (m.AddMethod?.IsFamily ?? false)) && !m.IsSpecialName))
                    {
                        eventResolver.Resolve(ev);
                    }
                }, cancellationToken)
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
