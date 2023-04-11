using NUnit.Framework;
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
            return AnyAttributeName<TestAttribute>(customAttributeProvider) || AnyAttributeName<TestCaseAttribute>(customAttributeProvider);
        }

        /// <summary>
        /// GetFilterTestMethods
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<MethodInfo> GetFilterTestMethods(Type type)
        {
            var methods = type.GetMethods().OrderBy(e => e.Name);
            var testMethods = methods.Where(AnyTestAttribute);
            var onlyTypeTests = testMethods.Where(m => GetTestAttributes(m).Any(a => HasFilterTestMethod(m, a)));
            //foreach (var testMethod in testMethods)
            //{
            //    //Debug.WriteLine($"{testMethod.Name} {testMethod.GetCustomAttributes(true).Count()}");
            //    Debug.WriteLine($"{testMethod.Name} {string.Join(" ", testMethod.GetCustomAttributes(true).Select(e => e.GetType().Name))}");
            //    Debug.WriteLine($"{testMethod.Name} {testMethod.GetCustomAttributes(true).OfType<TestAttribute>().Count()}");
            //}
            //Debug.WriteLine($"Debug:\t{type}:\t{string.Join(" ", onlyTypeTests.Select(e => e.Name))}");
            return onlyTypeTests;
        }

        /// <summary>
        /// HasFilterTestMethod
        /// </summary>
        /// <param name="method"></param>
        /// <param name="nUnitAttribute"></param>
        /// <returns></returns>
        public bool HasFilterTestMethod(MethodInfo method, NUnitAttribute nUnitAttribute)
        {
            return TestEngineFilter.HasName(GetTestFullName(method, nUnitAttribute));
        }

        /// <summary>
        /// GetMethodFullName
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public string GetMethodFullName(MethodInfo method)
        {
            return method.DeclaringType.FullName + "." + method.Name;
        }

        /// <summary>
        /// GetTestFullName
        /// </summary>
        /// <param name="method"></param>
        /// <param name="nUnitAttribute"></param>
        /// <returns></returns>
        public string GetTestFullName(MethodInfo method, NUnitAttribute nUnitAttribute)
        {
            return GetMethodFullName(method) + "." + GetTestName(method, nUnitAttribute);
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
            if (attribute is TestCaseAttribute testCaseAttribute)
            {
                var name = testCaseAttribute.TestName ?? $"{method.Name}({string.Join(",", testCaseAttribute.Arguments)})";
                return name;
            }

            var parameters = method.GetParameters();
            if (parameters.Any())
            {
                return $"{method.Name}({string.Join(",", parameters.Select(e => e.ParameterType.Name))})";
            }

            return method.Name;
        }

        /// <summary>
        /// GetMethodTestAttributes
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public IEnumerable<NUnitAttribute> GetTestAttributes(MethodInfo method)
        {
            var attributes = new List<NUnitAttribute>();
            if (TryGetAttributes<TestCaseAttribute>(method, out IEnumerable<TestCaseAttribute> testCases))
            {
                attributes.AddRange(testCases);
                return testCases.OrderBy(e => GetTestName(method, e));
            }
            var attribute = GetAttribute<TestAttribute>(method);
            if (attribute is not null) attributes.Add(attribute);

            return attributes;
        }
    }
}
