using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public static class TypeResolver
    {
        public static TypeMetadata Type(this IAssemblyMetadata metadata, in Type type)
        {
            TypeMetadata meta = metadata.Type(type.GetHashCode());

            meta.Name = type.Name;
            meta.Namespace = type.Namespace;
            meta.Assembly = type.Module.Name;
            meta.IsMicrosoftType = IsMicrosoftType(type);
            meta.Category = type.GetCategory();
            meta.Modifier = type.GetModifier();

            IEnumerable<Type> interfaces = type.GetImmediateInterfaces();
            foreach (Type interf in interfaces)
            {
                TypeMetadata interfMeta = metadata.Type(interf);
                meta.Implement(interfMeta);
            }

            Type? baseType = type.BaseType;
            if (baseType != null)
            {
                TypeMetadata baseMeta = metadata.Type(baseType);
                meta.Inherit(baseMeta);
            }

            return meta;
        }

        private static IEnumerable<Type> GetImmediateInterfaces(this Type type)
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

        public static TypeCategory GetCategory(this Type type)
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

            return TypeCategory.None;
        }

        public static TypeModifier GetModifier(this Type type)
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

        public static bool IsMicrosoftType(this Type type)
        {
            var attribute = type.Assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            return attribute?.Company.Contains("Microsoft Corporation", StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
