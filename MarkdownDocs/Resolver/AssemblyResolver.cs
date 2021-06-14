using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Resolver
{
    public static class AssemblyResolverExtensions
    {
        public static TypeMetadata RegisterType(this IMetadataBuilder builder, Type type) => builder.Type(type.GetHashCode()).From(builder, type);
    }

    public class AssemblyResolver : IAssemblyResolver
    {
        private readonly IMetadataBuilder _metaBuilder;

        public AssemblyResolver(IMetadataBuilder metaBuilder)
        {
            _metaBuilder = metaBuilder;

            _metaBuilder.RegisterType(typeof(object));
            _metaBuilder.RegisterType(typeof(bool));
            _metaBuilder.RegisterType(typeof(double));
            _metaBuilder.RegisterType(typeof(int));
            _metaBuilder.RegisterType(typeof(string));
        }

        public async Task<AssemblyMetadata> ResolveAsync(string path, CancellationToken cancellationToken)
        {
            Assembly? dll = Assembly.LoadFrom(path);

            IEnumerable<Task> tasks = dll.ExportedTypes.Select(type => VisitTypeAsync(type, cancellationToken));
            await Task.WhenAll(tasks);

            return _metaBuilder.Build();
        }

        private async Task VisitTypeAsync(Type type, CancellationToken cancellationToken)
        {
            TypeMetadata typeRef = _metaBuilder.RegisterType(type);

            var tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    foreach (ConstructorInfo ctor in type.GetConstructors())
                    {
                        ConstructorMetadata ctorRef = typeRef.Constructor(ctor.GetHashCode());
                        VisitConstructor(ctorRef, ctor);
                    }
                }, cancellationToken),

                Task.Run(() =>
                {
                    foreach (FieldInfo field in type.GetFields())
                    {
                        FieldMetadata fieldRef = typeRef.Field(field.GetHashCode());
                        VisitField(fieldRef, field);
                    }
                }, cancellationToken),

                Task.Run(() =>
                {
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        PropertyMetadata propRef = typeRef.Property(property.GetHashCode());
                        VisitProperty(propRef, property);
                    }
                }, cancellationToken),

                Task.Run(() =>
                {
                    foreach (MethodInfo method in type.GetMethods())
                    {
                        MethodMetadata methodRef = typeRef.Method(method.GetHashCode());
                        VisitMethod(methodRef, method);
                    }
                }, cancellationToken),

                Task.Run(() =>
                {
                    foreach (EventInfo ev in type.GetEvents())
                    {
                        EventMetadata eventRef = typeRef.Event(ev.GetHashCode());
                        VisitEvent(eventRef, ev);
                    }
                }, cancellationToken)
            };

            await Task.WhenAll(tasks);
        }

        private void VisitEvent(EventMetadata eventRef, EventInfo ev)
        {

        }

        private void VisitMethod(MethodMetadata methodRef, MethodInfo method)
        {

        }

        private void VisitProperty(PropertyMetadata propRef, PropertyInfo property)
        {

        }

        private void VisitField(FieldMetadata fieldRef, FieldInfo field)
        {

        }

        private void VisitConstructor(ConstructorMetadata ctorRef, ConstructorInfo ctor)
        {

        }
    }
}
