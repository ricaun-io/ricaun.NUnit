using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// TestAttributeService
    /// </summary>
    public class TestAttributeService : AttributeService
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
        /// GetMethodFullName
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public string GetMethodFullName(MethodInfo method)
        {
            return method.DeclaringType.FullName + "." + method.Name;
        }

        /// <summary>
        /// GetMethodTestNames
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public string[] GetMethodTestNames(MethodInfo method)
        {
            return GetMethodTestAttributes(method).Select(e => GetMethodTestName(method, e)).ToArray();
        }

        /// <summary>
        /// GetMethodTestName
        /// </summary>
        /// <param name="method"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public string GetMethodTestName(MethodInfo method, NUnitAttribute attribute)
        {
            if (attribute is TestCaseAttribute testCaseAttribute)
            {
                var name = testCaseAttribute.TestName ?? $"{method.Name}({string.Join(",", testCaseAttribute.Arguments)})";
                return name;
            }
            return method.Name;
        }

        /// <summary>
        /// GetMethodTestAttributes
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public IEnumerable<NUnitAttribute> GetMethodTestAttributes(MethodInfo method)
        {
            var attributes = new List<NUnitAttribute>();
            if (TryGetAttributes<TestCaseAttribute>(method, out IEnumerable<TestCaseAttribute> testCases))
            {
                attributes.AddRange(testCases);
                return testCases.OrderBy(e => GetMethodTestName(method, e));
            }
            var attribute = GetAttribute<TestAttribute>(method);
            if (attribute is not null) attributes.Add(attribute);

            return attributes;
        }

        /// <summary>
        /// GetTestMethods
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<MethodInfo> GetTestMethods(Type type)
        {
            var methods = type.GetMethods().OrderBy(e => e.Name);
            return methods.Where(AnyTestAttribute).Where(e => TestEngineFilter.HasName(e.Name));
        }

    }
}
