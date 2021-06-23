using MarkdownDocs.Context;
using MarkdownDocs.Markdown;
using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MarkdownDocs.Resolver
{
    public interface IXMLMemberResolver
    {
        Task ResolveAsync(IAssemblyContext context, XElement member, CancellationToken cancellationToken = default);
    }

    public class XMLMemberResolver : IXMLMemberResolver
    {
        private readonly IDocsUrlResolver _urlResolver;

        public XMLMemberResolver(IDocsUrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

        public Task ResolveAsync(IAssemblyContext context, XElement member, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string name = member.Attribute("name")!.Value;
            string startsWith = name[..2];

            switch (startsWith)
            {
                case "T:":
                    ResolveType(context, member);
                    break;

                case "P:":
                case "F:":
                case "E:":
                    ResolveMember(context, member);
                    break;

                case "M:":
                    ResolveMethod(context, member);
                    break;
            }

            return Task.CompletedTask;
        }

        private void ResolveType(IAssemblyContext context, XElement member)
        {
            string name = member.Attribute("name")!.Value;
            string cleanName = name[2..];
            (string Name, string? Namespace) = ExtractName(cleanName);

            IAssemblyMetadata meta = context.GetMetadata();
            ITypeMetadata? type = meta.Types.FirstOrDefault(t => t.Name == Name && t.Namespace == Namespace);

            if (type != null)
            {
                ITypeContext typeContext = context.Type(type.Id);
                XElement? summary = member.Element("summary");
                if (summary != null)
                {
                    string? description = ResolveText(context, typeContext, summary)?.Trim();
                    typeContext.Description = description;
                }

                if (typeContext.Category == TypeCategory.Delegate)
                {
                    IMethodMetadata methodMeta = type.Methods.First(m => m.Name == "Invoke");
                    IMethodContext methodContext = typeContext.Method(methodMeta.Id);

                    var paramsMembers = member.Elements("param").Union(member.Elements("paramref")).ToList();

                    foreach (var param in methodMeta.Parameters)
                    {
                        var paramMember = paramsMembers.FirstOrDefault(p => p.Attribute("name")?.Value == param.Name);
                        if (paramMember != null)
                        {
                            IParameterContext paramContext = methodContext.Parameter(param.Id);
                            paramContext.Description = ResolveText(context, typeContext, paramMember)?.Trim();
                        }
                    }
                }
            }
        }

        private void ResolveMember(IAssemblyContext context, XElement member)
        {
            string name = member.Attribute("name")!.Value;
            string cleanName = name[2..];
            (string MemberName, string? MemberPath) = ExtractName(cleanName);
            (string TypeName, string? Namespace) = ExtractName(MemberPath ?? string.Empty);

            IAssemblyMetadata meta = context.GetMetadata();
            ITypeMetadata? type = meta.Types.FirstOrDefault(t => t.Name == TypeName && t.Namespace == Namespace);

            if (type != null)
            {
                ITypeContext typeContext = context.Type(type.Id);

                XElement? summary = member.Element("summary");
                if (summary != null)
                {
                    string? description = ResolveText(context, typeContext, summary)?.Trim();

                    IDocMetadata? memberMeta = GetMembers(type).FirstOrDefault(m => m.Name == MemberName);
                    // There has to be another way
                    IDocContext? memberContext = memberMeta as IDocContext;

                    if (memberContext != null)
                    {
                        memberContext.Description = description;
                    }
                }
            }
        }

        private void ResolveMethod(IAssemblyContext context, XElement member)
        {
            string name = member.Attribute("name")!.Value;
            int parenStart = name.IndexOf("(");
            string cleanName = name[2..];

            string[] parameterTypes = Array.Empty<string>();
            if (parenStart != -1)
            {
                cleanName = name[2..parenStart];
                string paramsPart = name[(parenStart + 1)..^1];
                parameterTypes = paramsPart.Split(",");
            }

            (string MemberName, string? MemberPath) = ExtractName(cleanName);
            (string TypeName, string? Namespace) = ExtractName(MemberPath ?? string.Empty);

            IAssemblyMetadata meta = context.GetMetadata();
            ITypeMetadata? type = meta.Types.FirstOrDefault(t => t.Name == TypeName && t.Namespace == Namespace);

            if (type != null)
            {
                ITypeContext typeContext = context.Type(type.Id);
                List<IParameterizedMemberMetadata> methods = type.Methods.Where(m => m.Name == MemberName).Cast<IParameterizedMemberMetadata>().ToList();

                if (MemberName == "#ctor")
                {
                    methods = type.Constructors.Cast<IParameterizedMemberMetadata>().ToList();
                }

                IParameterizedMemberMetadata? methodMeta = methods.Select(m => new
                {
                    Method = m,
                    Parameters = m.Parameters.Select(p => $"{p.Type.Namespace}.{p.Type.Name}")
                }).FirstOrDefault(m => !m.Parameters.Except(parameterTypes).Any())?.Method
                ?? methods.FirstOrDefault(m => m.Parameters.Count() == parameterTypes.Length);

                if (methodMeta != null)
                {
                    IMethodBaseContext methodContext = MemberName == "#ctor" ? typeContext.Constructor(methodMeta.Id) : typeContext.Method(methodMeta.Id);
                    XElement? summary = member.Element("summary");
                    if (summary != null)
                    {
                        string? description = ResolveText(context, typeContext, summary)?.Trim();
                        methodContext.Description = description;
                    }

                    if (methodContext is IMethodContext methodC)
                    {
                        XElement? returns = member.Element("returns");
                        if (returns != null)
                        {
                            string? description = ResolveText(context, typeContext, returns)?.Trim();
                            methodC.ReturnDescription = description;
                        }
                    }

                    List<XElement> paramsMembers = member.Elements("param").Union(member.Elements("paramref")).ToList();

                    foreach (var param in methodMeta.Parameters)
                    {
                        var paramMember = paramsMembers.FirstOrDefault(p => p.Attribute("name")?.Value == param.Name);
                        if (paramMember != null)
                        {
                            IParameterContext paramContext = methodContext.Parameter(param.Id);
                            paramContext.Description = ResolveText(context, typeContext, paramMember)?.Trim();
                        }
                    }
                }
            }
        }

        private string AddReferenceAndGenerateLink(IAssemblyContext context, ITypeContext? typeContext, string xmlMember)
        {
            IAssemblyMetadata meta = context.GetMetadata();
            string cleanName = xmlMember[2..];
            (string Name, string? MemberPath) = ExtractName(cleanName);

            string prefix = xmlMember[..2];
            switch (prefix)
            {
                case "T:":
                    ITypeMetadata? type = meta.Types.FirstOrDefault(t => t.Name == Name && t.Namespace == MemberPath);
                    if (type != null && typeContext != null)
                    {
                        ITypeContext reference = context.Type(type.Id);
                        typeContext.Reference(reference);
                        return type.Link(typeContext.GetMetadata(), _urlResolver);
                    }
                    break;

                case "P:":
                case "F:":
                case "E:":
                    (string TypeName, string? Namespace) = ExtractName(MemberPath ?? string.Empty);
                    ITypeMetadata? owner = meta.Types.FirstOrDefault(t => t.Name == TypeName && t.Namespace == Namespace);
                    if (owner != null && typeContext != null)
                    {
                        IMemberMetadata? memberMeta = GetMembers(owner).FirstOrDefault(m => m.Name == Name);
                        if (memberMeta != null)
                        {
                            ITypeContext reference = context.Type(owner.Id);
                            typeContext.Reference(reference);
                            return memberMeta.Link(typeContext.GetMetadata(), _urlResolver);
                        }

                        return Name.Link(owner, typeContext.GetMetadata(), _urlResolver);
                    }
                    break;
            }

            return cleanName;
        }

        private string? ResolveText(IAssemblyContext context, ITypeContext? typeContext, XElement member)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var node in member.Nodes())
            {
                if (node is XText text)
                {
                    builder.Append(text.Value);
                }
                else if (node is XElement elem)
                {
                    XAttribute? cref = elem.Attribute("cref");
                    XAttribute? href = elem.Attribute("href");
                    XAttribute? name = elem.Attribute("name");

                    if (cref != null)
                    {
                        string link = AddReferenceAndGenerateLink(context, typeContext, cref.Value);
                        builder.Append(link);
                    }
                    else if (href != null)
                    {
                        string link = AddReferenceAndGenerateLink(context, typeContext, href.Value);
                        builder.Append(link);
                    }
                    else if (name != null)
                    {
                        builder.Append(name.Value);
                    }
                    else
                    {
                        builder.Append(elem.Value);
                    }
                }
            }

            return builder.ToString();
        }

        private (string Name, string? Namespace) ExtractName(string cleanName)
        {
            int typeStart = cleanName.LastIndexOf(".");
            if (typeStart != -1)
            {
                string typeNamespace = cleanName.Substring(0, typeStart);
                string typeName = cleanName.Substring(typeStart + 1, cleanName.Length - typeStart - 1);
                return (typeName, typeNamespace);
            }
            return (cleanName, null);
        }

        private IEnumerable<IMemberMetadata> GetMembers(ITypeMetadata type)
        {
            var ctors = type.Constructors.Cast<IMemberMetadata>();
            var fields = type.Fields.Cast<IMemberMetadata>();
            var props = type.Properties.Cast<IMemberMetadata>();
            var events = type.Events.Cast<IMemberMetadata>();

            return ctors.Union(fields).Union(props).Union(events);
        }
    }
}
