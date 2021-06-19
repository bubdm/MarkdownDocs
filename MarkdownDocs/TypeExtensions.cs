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

        public static string ToLiteralString(this object? value)
        {
            if (value != null)
            {
                Type type = value.GetType();

                if (type == typeof(bool))
                {
                    return value is true ? "true" : "false";
                }

                if (type == typeof(string))
                {
                    return $"\"{value}\"";
                }

                if (type == typeof(char))
                {
                    return $"'{value}'";
                }

                if (type == typeof(uint))
                {
                    return $"{value}u";
                }

                if (type == typeof(long))
                {
                    return $"{value}l";
                }

                if (type == typeof(ulong))
                {
                    return $"{value}ul";
                }

                if (type == typeof(float))
                {
                    return $"{value}f";
                }

                if (type == typeof(double))
                {
                    return $"{value}d";
                }

                if (type == typeof(decimal))
                {
                    return $"{value}m";
                }

                return value.ToString() ?? "null";
            }

            return "null";
        }
    }
}
