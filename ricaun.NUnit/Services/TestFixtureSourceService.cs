using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ricaun.NUnit.Services
{
    internal static class TestFixtureSourceService
    {
        /// <summary>
        /// Get TestFixtureAttribute from TestFixtureSourceAttribute
        /// </summary>
        /// <param name="type"></param>
        /// <param name="testFixtureSource"></param>
        /// <returns></returns>
        public static IEnumerable<TestFixtureAttribute> GetTestFixtureFromSource(Type type, TestFixtureSourceAttribute testFixtureSource)
        {
            var attributes = new List<TestFixtureAttribute>();
            try
            {
                IEnumerable source = GetTestFixtureSource(type, testFixtureSource);
                if (source is not null)
                {
                    foreach (object item in source)
                    {
                        var testFixture = new TestFixtureAttribute(item);
                        if (item is Array array)
                        {
                            var args = new object[array.Length];
                            for (var i = 0; i < array.Length; i++)
                                args[i] = array.GetValue(i);

                            testFixture = new TestFixtureAttribute(args);
                        }
                        else if (item is TestFixtureData testFixtureData)
                        {
                            testFixture = new TestFixtureAttribute(testFixtureData.Arguments);
                        }

                        attributes.Add(testFixture);
                    }
                }
            }
            catch { }

            return attributes;
        }

        /// <summary>
        /// GetTestFixtureSource
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        /// <remarks>
        /// Based: GetTestFixtureSource method from NUnit.Framework.Internal.TestFixtureSourceAttribute
        /// https://github.com/nunit/nunit/blob/v3.13/src/NUnitFramework/framework/Attributes/TestFixtureSourceAttribute.cs
        /// </remarks>
        internal static IEnumerable GetTestFixtureSource(Type sourceType, TestFixtureSourceAttribute attribute)
        {
            // Handle Type implementing IEnumerable separately
            if (attribute.SourceName is null)
                return Reflect.Construct(sourceType) as IEnumerable;

            MemberInfo[] members = sourceType.GetMemberIncludingFromBase(attribute.SourceName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            if (members.Length == 1)
            {
                MemberInfo member = members[0];

                var field = member as FieldInfo;
                if (field != null)
                    return field.IsStatic
                        ? (IEnumerable)field.GetValue(null)
                        : SourceMustBeStaticError();

                var property = member as PropertyInfo;
                if (property != null)
                    return property.GetGetMethod(true).IsStatic
                        ? (IEnumerable)property.GetValue(null, null)
                        : SourceMustBeStaticError();

                var m = member as MethodInfo;
                if (m != null)
                    return m.IsStatic
                        ? (IEnumerable)m.Invoke(null, null)
                        : SourceMustBeStaticError();
            }

            return null;
        }

        private static IEnumerable SourceMustBeStaticError()
        {
            return "SourceMustBeStaticError";
        }

    }
}
