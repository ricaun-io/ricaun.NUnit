using NUnit.Framework;
using ricaun.NUnit.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// TestAttributeService
    /// </summary>
    internal class TestAttributeService : AttributeService
    {
        /// <summary>
        /// AnyTestAttribute
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <returns></returns>
        public bool AnyTestAttribute(ICustomAttributeProvider customAttributeProvider)
        {
            try
            {
                return AnyAttributeName(customAttributeProvider, typeof(TestAttribute), typeof(TestCaseAttribute), typeof(TestCaseSourceAttribute));
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
            return false;
        }

        /// <summary>
        /// OrderTestAttribute number
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <returns></returns>
        public int OrderTestAttribute(ICustomAttributeProvider customAttributeProvider)
        {
            try
            {
                if (TryGetAttribute<OrderAttribute>(customAttributeProvider, out OrderAttribute orderAttribute))
                {
                    return orderAttribute.Order;
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
            return 0;
        }

        /// <summary>
        /// AnyMethodWithTestAttribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool AnyMethodWithTestAttribute(Type type)
        {
            return GetMethodWithTestAttribute(type).Any();
        }

        /// <summary>
        /// GetMethodWithTestAttribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<MethodInfo> GetMethodWithTestAttribute(Type type)
        {
            return type.GetMethods().Where(AnyTestAttribute).OrderBy(e => e.Name).OrderBy(OrderTestAttribute);
        }

        /// <summary>
        /// AnyMethodWithTestAttributeAndFilter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Obsolete]
        public bool AnyMethodWithTestAttributeAndFilter(Type type)
        {
            return GetMethodWithTestAttributeAndFilter(type).Any();
        }

        public bool AnyMethodWithTestAttributeAndFilter(TypeInstance type)
        {
            return GetMethodWithTestAttributeAndFilter(type).Any();
        }

        public IEnumerable<MethodInfo> GetMethodWithTestAttributeAndFilter(TypeInstance type)
        {
            return GetMethodWithTestAttribute(type).Where(m => GetTestAttributes(m).Any(a => HasFilterTestMethod(type.FullName, m, a)));
        }

        /// <summary>
        /// GetMethodWithTestAttributeAndFilter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Obsolete]
        public IEnumerable<MethodInfo> GetMethodWithTestAttributeAndFilter(Type type)
        {
            return GetMethodWithTestAttribute(type).Where(m => GetTestAttributes(m).Any(a => HasFilterTestMethod(type, m, a)));
        }

        /// <summary>
        /// HasFilterTestMethod
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <param name="nUnitAttribute"></param>
        /// <returns></returns>
        [Obsolete]
        public bool HasFilterTestMethod(Type type, MethodInfo method, NUnitAttribute nUnitAttribute)
        {
            return TestEngineFilter.HasName(GetTestFullName(type, method, nUnitAttribute));
        }

        public bool HasFilterTestMethod(string fullName, MethodInfo method, NUnitAttribute nUnitAttribute)
        {
            return TestEngineFilter.HasName(GetTestFullName(fullName, method, nUnitAttribute));
        }

        /// <summary>
        /// GetMethodFullName
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        [Obsolete]
        public string GetMethodFullName(Type declaringType, MethodInfo method)
        {
            return GetMethodFullName(declaringType.FullName, method);
        }

        public string GetMethodFullName(TypeInstance typeInstance, MethodInfo method)
        {
            return typeInstance + "." + method.Name;
        }

        public string GetTestFullName(TypeInstance typeInstance, MethodInfo method, NUnitAttribute nUnitAttribute)
        {
            return GetMethodFullName(typeInstance, method) + "." + GetTestName(method, nUnitAttribute);
        }

        /// <summary>
        /// GetMethodFullName
        /// </summary>
        /// <param name="fullNameType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public string GetMethodFullName(string fullNameType, MethodInfo method)
        {
            return fullNameType + "." + method.Name;
        }

        /// <summary>
        /// GetTestFullName
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <param name="nUnitAttribute"></param>
        /// <returns></returns>
        [Obsolete]
        public string GetTestFullName(Type type, MethodInfo method, NUnitAttribute nUnitAttribute)
        {
            return GetMethodFullName(type, method) + "." + GetTestName(method, nUnitAttribute);
        }

        /// <summary>
        /// GetTestFullName
        /// </summary>
        /// <param name="fullNameType"></param>
        /// <param name="method"></param>
        /// <param name="nUnitAttribute"></param>
        /// <returns></returns>
        public string GetTestFullName(string fullNameType, MethodInfo method, NUnitAttribute nUnitAttribute)
        {
            return GetMethodFullName(fullNameType, method) + "." + GetTestName(method, nUnitAttribute);
        }

        /// <summary>
        /// GetMethodTestNames
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public string[] GetMethodTestNames(MethodInfo method)
        {
            return GetTestAttributes(method).Select(e => GetTestName(method, e)).ToArray();
        }

        /// <summary>
        /// GetMethodTestName
        /// </summary>
        /// <param name="method"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public string GetTestName(MethodInfo method, NUnitAttribute attribute)
        {
            try
            {
                if (attribute is TestCaseAttribute testCaseAttribute)
                {
                    return testCaseAttribute.TestName ?? GetTestNameWithArguments(method, testCaseAttribute.Arguments);
                }

                var parameters = method.GetParameters();
                if (parameters.Any())
                {
                    return GetTestNameWithArguments(method, parameters);
                }
            }
            catch { return $"{method.Name}(*)"; }

            return method.Name;
        }

        static string GetTestNameWithArguments(MethodInfo method, params object[] objects)
        {
            return $"{method.Name}{objects.ToArgumentName()}";
        }

        /// <summary>
        /// GetMethodTestAttributes
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public IEnumerable<NUnitAttribute> GetTestAttributes(MethodInfo method)
        {
            if (TryGetAttributes<TestCaseAttribute>(method, out IEnumerable<TestCaseAttribute> testCases))
            {
                return testCases.OrderBy(e => GetTestName(method, e));
            }
            if (TryGetAttribute<TestCaseSourceAttribute>(method, out TestCaseSourceAttribute testCaseSource))
            {
                return TestCaseSourceService.GetTestCasesFromSource(method, testCaseSource);
            }
            if (GetAttribute<TestAttribute>(method) is TestAttribute attribute)
            {
                return new[] { attribute };
            }
            return Enumerable.Empty<NUnitAttribute>();
        }

        /// <summary>
        /// Retrieves the test fixture attributes for a given type.
        /// </summary>
        /// <param name="type">The type to retrieve the test fixture attributes from.</param>
        /// <returns>An enumerable of <see cref="TestFixtureAttribute"/> instances.</returns>
        public IEnumerable<TestFixtureAttribute> GetTestFixtureAttributes(Type type)
        {
            if (TryGetAttributes<TestFixtureAttribute>(type, out IEnumerable<TestFixtureAttribute> testFixtures))
            {
                return testFixtures;
            }
            if (TryGetAttribute<TestFixtureSourceAttribute>(type, out TestFixtureSourceAttribute testFixtureSource))
            {
                return TestFixtureSourceService.GetTestFixtureFromSource(type, testFixtureSource);
            }
            return new[] { new TestFixtureAttribute() };
        }
    }
}
