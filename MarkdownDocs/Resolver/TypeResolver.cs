﻿using MarkdownDocs.Metadata;
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
        private readonly IAssemblyContext _assemblyBuilder;
        private readonly Func<ITypeResolver, ITypeContext, IMethodResolver> _methodResolver;

        public TypeResolver(IAssemblyContext assemblyBuilder, Func<ITypeResolver, ITypeContext, IMethodResolver> methodResolver)
        {
            _assemblyBuilder = assemblyBuilder;
            _methodResolver = methodResolver;
        }

        public ITypeContext Resolve(Type type)
        {
            ITypeContext typeMeta = ResolveRecursive(type);

            return typeMeta;
        }

        // TODO: Return from cache if already resolved
        private ITypeContext ResolveRecursive(Type type)
        {
            Type? realType = Nullable.GetUnderlyingType(type);
            type = realType ?? type;

            ITypeContext meta = _assemblyBuilder.Type(type.GetHashCode());
            
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
                meta.Implement(interfMeta);
            }

            Type? baseType = type.BaseType;
            if (baseType != null)
            {
                ITypeContext baseMeta = ResolveRecursive(baseType);
                meta.Inherit(baseMeta);
            }

            if (meta.Category == TypeCategory.Delegate)
            {
                MethodInfo? invoke = type.GetMethod("Invoke");
                if (invoke != null)
                {
                    IMethodResolver resolver = _methodResolver(this, meta);
                    resolver.Resolve(invoke);
                }
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

        public static string? GetCompanyName(Type type)
        {
            var attribute = type.Assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            return attribute?.Company;
        }
    }
}
