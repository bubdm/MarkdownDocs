using MarkdownDocs.Context;
using MarkdownDocs.Metadata;
using MarkdownDocs.Resolver;
using System;
using Xunit;

namespace MarkdownDocs.Tests
{
    #region Stubs

    enum TestEnum
    {
        None
    }

    interface IBase
    {

    }

    interface IDerived
    {

    }

    class BaseClass : IBase
    {
        public BaseClass? Root { get; set; }
    }

    class DerivedClass : BaseClass, IDerived
    {

    }

    #endregion

    public static class TypeMetaExtensions
    {
        public static bool IsMicrosoftType(this ITypeMetadata meta) => meta.Company == DocsUrlResolver.Microsoft;
    }

    public class TypeResolverTests
    {
        private readonly ITypeResolver _typeResolver;

        public TypeResolverTests()
        {
            static IParameterResolver ParameterResolverFactory(IMethodContext context, ITypeResolver typeResolver) => new ParameterResolver(context, typeResolver);
            static IMethodResolver MethodResolverFactory(ITypeResolver typeResolver, ITypeContext context) => new MethodResolver(typeResolver, context, ParameterResolverFactory);
            IAssemblyContext builder = new AssemblyMetadataStub();
            _typeResolver = new TypeResolver(builder, MethodResolverFactory);
        }

        [Fact]
        public void TestBoolType()
        {
            Type type = typeof(bool);
            ITypeContext meta = _typeResolver.Resolve(type);

            Assert.Equal(type.Name, meta.Name);
            Assert.Equal(type.Namespace, meta.Namespace);
            Assert.Equal(type.Assembly.GetName().Name, meta.Assembly);
            Assert.True(meta.IsMicrosoftType());
            Assert.Equal(TypeCategory.Struct, meta.Category);
            Assert.Equal(TypeModifier.None, meta.Modifier);
        }

        [Fact]
        public void TestEnumType()
        {
            Type type = typeof(TestEnum);
            ITypeContext meta = _typeResolver.Resolve(type);

            Assert.Equal(type.Name, meta.Name);
            Assert.Equal(type.Namespace, meta.Namespace);
            Assert.Equal(type.Assembly.GetName().Name, meta.Assembly);
            Assert.False(meta.IsMicrosoftType());
            Assert.Equal(TypeCategory.Enum, meta.Category);
            Assert.Equal(TypeModifier.None, meta.Modifier);
            Assert.Empty(meta.Implemented);
        }

        [Fact]
        public void TestInheritance()
        {
            ITypeContext meta = _typeResolver.Resolve(typeof(DerivedClass));

            Assert.Same(_typeResolver.Resolve(typeof(BaseClass)), meta.Inherited);
            Assert.Contains(_typeResolver.Resolve(typeof(IDerived)), meta.Implemented);
            Assert.DoesNotContain(_typeResolver.Resolve(typeof(IBase)), meta.Implemented);
            Assert.Empty(meta.Derived);
        }

        [Fact]
        public void TestDerived()
        {
            ITypeContext meta = _typeResolver.Resolve(typeof(BaseClass));

            Assert.Contains(_typeResolver.Resolve(typeof(DerivedClass)), meta.Derived);
        }
    }
}
