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
            ITypeContext context = _typeResolver.Resolve(type);
            ITypeMetadata meta = context.GetMetadata();

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
            ITypeContext context = _typeResolver.Resolve(type);
            ITypeMetadata meta = context.GetMetadata();

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
            ITypeContext context = _typeResolver.Resolve(typeof(DerivedClass));
            ITypeMetadata meta = context.GetMetadata();

            ITypeMetadata inherited = _typeResolver.Resolve(typeof(BaseClass)).GetMetadata();
            Assert.Same(inherited, meta.Inherited);

            ITypeMetadata derivedInterface = _typeResolver.Resolve(typeof(IDerived)).GetMetadata();
            Assert.Contains(derivedInterface, meta.Implemented);

            ITypeMetadata baseInterface = _typeResolver.Resolve(typeof(IBase)).GetMetadata();
            Assert.DoesNotContain(baseInterface, meta.Implemented);
            Assert.Empty(meta.Derived);
        }

        [Fact]
        public void TestDerived()
        {
            ITypeMetadata meta = _typeResolver.Resolve(typeof(BaseClass)).GetMetadata();
            ITypeMetadata derived = _typeResolver.Resolve(typeof(DerivedClass)).GetMetadata();

            Assert.Contains(derived, meta.Derived);
        }
    }
}
