using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public interface IResolver<TResult, TValue>
    {
        TResult Resolve(in TValue value);
    }

    public class TypeResolver : IResolver<ITypeMetadata, Type>
    {
        private readonly IAssemblyBuilder _builder;

        public TypeResolver(in IAssemblyBuilder builder)
        {
            _builder = builder;
        }

        public ITypeMetadata Resolve(in Type type) => ResolveImpl(type);

        private TypeMetadata ResolveImpl(in Type type)
        {
            TypeMetadata meta = _builder.Type(type.GetHashCode());

            meta.Name = type.Name;
            meta.Namespace = type.Namespace;
            meta.Assembly = type.Assembly.GetName().Name;
            meta.IsMicrosoftType = IsMicrosoftType(type);
            meta.Category = GetCategory(type);
            meta.Modifier = GetModifier(type);

            IEnumerable<Type> interfaces = GetImmediateInterfaces(type);
            foreach (Type interf in interfaces)
            {
                TypeMetadata interfMeta = ResolveImpl(interf);
                meta.Implement(interfMeta);
            }

            Type? baseType = type.BaseType;
            if (baseType != null)
            {
                TypeMetadata baseMeta = ResolveImpl(baseType);
                meta.Inherit(baseMeta);
            }

            return meta;
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

        public static bool IsMicrosoftType(Type type)
        {
            var attribute = type.Assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            return attribute?.Company.Contains("Microsoft Corporation", StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
