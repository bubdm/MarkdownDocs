using MarkdownDocs.Context;
using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public interface ITypeResolver
    {
        ITypeContext Resolve(Type value);
    }

    public class TypeResolver : ITypeResolver
    {
        private readonly IAssemblyContext _assemblyContext;
        private readonly Func<ITypeContext, ITypeResolver, IMethodResolver> _methodResolver;

        public TypeResolver(IAssemblyContext assemblyContext, Func<ITypeContext, ITypeResolver, IMethodResolver> methodResolver)
        {
            _assemblyContext = assemblyContext;
            _methodResolver = methodResolver;
        }

        public ITypeContext Resolve(Type type)
        {
            if (type.ContainsGenericParameters && type.FullName == null)
            {
                // If it's a generic parameter or generic return type resolve it in another context
                ITypeContext context = CreateContext().ResolveRecursive(type);
                ITypeMetadata meta = context.GetMetadata();

                meta.Assembly = string.Empty;
                return context;
            }

            ITypeContext typeMeta = ResolveRecursive(type);
            return typeMeta;
        }

        private ITypeContext ResolveRecursive(Type type)
        {
            ITypeContext context = _assemblyContext.Type(type.GetHashCode());
            ITypeMetadata meta = context.GetMetadata();

            // Type was not previously resolved
            if (string.IsNullOrEmpty(meta.Name))
            {
                meta.Name = type.ToPrettyName();
                meta.Namespace = type.Namespace;
                meta.Assembly = type.Assembly.GetName().Name;
                meta.Company = GetCompanyName(type);
                meta.Category = GetCategory(type);
                meta.Modifier = GetModifier(type);

                IEnumerable<Type> interfaces = GetImmediateInterfaces(type);
                foreach (Type interf in interfaces)
                {
                    ITypeContext interfMeta = ResolveRecursive(interf);
                    context.Implement(interfMeta);
                }

                Type? baseType = type.BaseType;
                if (baseType != null)
                {
                    ITypeContext baseMeta = ResolveRecursive(baseType);
                    context.Inherit(baseMeta);
                }

                if (meta.Category == TypeCategory.Delegate)
                {
                    MethodInfo? invoke = type.GetMethod("Invoke");
                    if (invoke != null)
                    {
                        IMethodResolver resolver = _methodResolver(context, this);
                        resolver.Resolve(invoke);
                    }
                }
            }

            return context;
        }

        private static IEnumerable<Type> GetImmediateInterfaces(Type type)
        {
            Type[] interfaces = type.GetInterfaces();
            var result = new HashSet<Type>(interfaces);
            if (type.BaseType != null)
            {
                var others = type.BaseType.GetInterfaces();
                result.ExceptWith(others);
            }

            foreach (Type interf in interfaces)
            {
                var others = interf.GetInterfaces();
                result.ExceptWith(others);
            }


            return result;
        }

        public static TypeCategory GetCategory(Type type)
        {
            if (type.IsEnum)
            {
                return TypeCategory.Enum;
            }

            if (type.IsInterface)
            {
                return TypeCategory.Interface;
            }

            if (type.IsValueType)
            {
                return TypeCategory.Struct;
            }

            if (type.IsSubclassOf(typeof(Delegate)))
            {
                return TypeCategory.Delegate;
            }

            if (type.IsClass)
            {
                return TypeCategory.Class;
            }

            return TypeCategory.Unknown;
        }

        public static TypeModifier GetModifier(Type type)
        {
            if (type.IsAbstract && type.IsSealed)
            {
                return TypeModifier.Static;
            }

            if (type.IsAbstract)
            {
                return TypeModifier.Abstract;
            }

            if (type.IsSealed && !type.IsValueType)
            {
                return TypeModifier.Sealed;
            }

            return TypeModifier.None;
        }

        public static string? GetCompanyName(Type type)
        {
            var attribute = type.Assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            return attribute?.Company;
        }

        public TypeResolver CreateContext() => new TypeResolver(new AssemblyContext(), _methodResolver);
    }
}
