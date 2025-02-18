using System;
using System.Linq;
using System.Reflection;

namespace ricaun.NUnit.Extensions
{
    internal static class ArgumentsExtension
    {
        /// <summary>
        /// Converts an array of objects to a formatted string representation.
        /// </summary>
        /// <param name="values">The array of objects to convert.</param>
        /// <returns>A string representation of the array of objects.</returns>
        /// <remarks>
        /// <code>
        /// [] => ""
        /// [1] => "(1)"
        /// [1, 2] => "(1,2)"
        /// [1, 2, "3"] => "(1,2,"3")"
        /// </code>
        /// </remarks>
        public static string ToArgumentName(this object[] values)
        {
            if (values is null) return string.Empty;
            if (values.Length == 0) return string.Empty;
            return $"({string.Join(",", values.Select(ValueToArgumentName))})";
        }
        public static string ValueToArgumentName(object value)
        {
            if (value is null)
            {
                return "null";
            }
            else if (value is string)
            {
                return $"\"{value}\"";
            }
            else if (value is float)
            {
                return $"{value}f";
            }
            else if (value is Type type)
            {
                try
                {
                    if (type.IsGenericType)
                    {
                        var genericTypeName = type.Name.Split('`')[0];
                        var genericArgs = type
                            .GetGenericArguments()
                            .Select(ValueToArgumentName);
                        return $"{genericTypeName}<{string.Join(",", genericArgs)}>";
                    }
                }
                catch { }
                return type.Name;
            }
            else if (value is ParameterInfo parameter)
            {
                var parameterType = parameter.ParameterType;
                return ValueToArgumentName(parameterType);
            }
            return $"{value}";
        }
    }
}
