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
        public BaseClass Root { get; set; }
    }

    class DerivedClass : BaseClass, IDerived
    {

    }

    #endregion

    public class TypeResolverTests
    {
        private readonly IMetadataBuilder _metadata = new MetaBuilderStub();

        [Fact]
        public void TestBoolType()
        {
            Type type = typeof(bool);
            TypeMetadata meta = _metadata.RegisterType(type);

            Assert.Equal(type.Name, meta.Name);
            Assert.Equal(type.Namespace, meta.Namespace);
            Assert.Equal(type.Module.Name, meta.Assembly);
            Assert.True(meta.IsMicrosoftType);
            Assert.Equal(TypeCategory.Struct, meta.Category);
            Assert.Equal(TypeModifier.None, meta.Modifier);
        }

        [Fact]
        public void TestEnumType()
        {
            Type type = typeof(TestEnum);
            TypeMetadata meta = _metadata.RegisterType(type);

            Assert.Equal(type.Name, meta.Name);
            Assert.Equal(type.Namespace, meta.Namespace);
            Assert.Equal(type.Module.Name, meta.Assembly);
            Assert.False(meta.IsMicrosoftType);
            Assert.Equal(TypeCategory.Enum, meta.Category);
            Assert.Equal(TypeModifier.None, meta.Modifier);
        }

        [Fact]
        public void TestInheritance()
        {
            TypeMetadata meta = _metadata.RegisterType(typeof(DerivedClass));

            Assert.Contains(_metadata.RegisterType(typeof(BaseClass)), meta.Inherited);
            Assert.Contains(_metadata.RegisterType(typeof(IDerived)), meta.Inherited);
            Assert.DoesNotContain(_metadata.RegisterType(typeof(IBase)), meta.Inherited);
            Assert.Empty(meta.Derived);
        }

        [Fact]
        public void TestDerived()
        {
            TypeMetadata derived = _metadata.RegisterType(typeof(DerivedClass));
            TypeMetadata meta = _metadata.RegisterType(typeof(BaseClass));

            Assert.Contains(_metadata.RegisterType(typeof(DerivedClass)), meta.Derived);
        }
    }
}
