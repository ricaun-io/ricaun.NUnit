using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// This class have the implementation to convert TestCaseSourceAttribute to TestCaseAttribute
    /// </summary>
    internal static class TestCaseSourceService
    {
        internal static IEnumerable<NUnitAttribute> GetTestCasesFromSource(MethodInfo method, TestCaseSourceAttribute testCaseSource)
        {
            var attributes = new List<NUnitAttribute>();
            try
            {
                IEnumerable source = GetTestCaseSource(method, testCaseSource);
                if (source is not null)
                {
                    foreach (object item in source)
                    {
                        attributes.Add(new TestCaseAttribute(item));
                    }
                }
            }
            catch { }

            return attributes;
        }

        /// <summary>
        /// GetTestCaseSource
        /// </summary>
        /// <param name="method"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        /// <remarks>
        /// Based: GetTestCaseSource method from NUnit.Framework.Internal.TestCaseSourceAttribute
        /// https://github.com/nunit/nunit/blob/main/src/NUnitFramework/framework/Attributes/TestCaseSourceAttribute.cs
        /// </remarks>
        internal static IEnumerable GetTestCaseSource(MethodInfo method, TestCaseSourceAttribute attribute)
        {
            Type sourceType = attribute.SourceType ?? method.DeclaringType;
            string SourceName = attribute.SourceName;

            if (SourceName is null)
            {
                return Reflect.Construct(sourceType, null) as IEnumerable;
            }

            MemberInfo[] members = sourceType.GetMemberIncludingFromBase(SourceName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            if (members.Length == 1)
            {
                MemberInfo member = members[0];

                var field = member as FieldInfo;
                if (field is not null)
                {
                    return field.IsStatic
                        ? (attribute.MethodParams is null ? (IEnumerable)field.GetValue(null)
                        : "ReturnErrorAsParameter") : "ReturnErrorAsParameter";
                }

                var property = member as PropertyInfo;
                if (property is not null)
                {
                    MethodInfo getMethod = property.GetGetMethod(true);
                    return getMethod?.IsStatic is true
                        ? (attribute.MethodParams is null ? (IEnumerable)property.GetValue(null, null)
                        : "ReturnErrorAsParameter") : "ReturnErrorAsParameter";
                }

                var m = member as MethodInfo;
                if (m is not null)
                {
                    return m.IsStatic
                        ? (attribute.MethodParams is null || m.GetParameters().Length == attribute.MethodParams.Length
                        ? (IEnumerable)m.Invoke(null, attribute.MethodParams)
                        : "ReturnErrorAsParameter") : "ReturnErrorAsParameter";
                }
            }

            return null;
        }

    }
}
