using System;
using System.Linq;

namespace MarkdownDocs
{
    public static class TypeExtensions
    {
        public static string ToPrettyName(this Type type)
        {
            if (type.IsArray)
            {
                Type? elemType = type.GetElementType();
                if (elemType != null)
                {
                    return $"{elemType.ToPrettyName()}[]";
                }
            }

            if (type.IsGenericType)
            {
                return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(ToPrettyName).ToArray()) + ">";
            }

            return type.Name;
        }
    }
}
