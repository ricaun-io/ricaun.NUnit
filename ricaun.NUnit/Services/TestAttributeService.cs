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
        public bool AnyMethodWithTestAttributeAndFilter(Type type)
        {
            return GetMethodWithTestAttributeAndFilter(type).Any();
        }

        /// <summary>
        /// GetMethodWithTestAttributeAndFilter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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
        public bool HasFilterTestMethod(Type type, MethodInfo method, NUnitAttribute nUnitAttribute)
        {
            return TestEngineFilter.HasName(GetTestFullName(type, method, nUnitAttribute));
        }

        /// <summary>
        /// GetMethodFullName
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public string GetMethodFullName(Type declaringType, MethodInfo method)
        {
            return GetMethodFullName(declaringType.FullName, method);
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

        private string GetTestNameWithArguments(MethodInfo method, params object[] objects)
        {
            string ToArgumentName(object value)
            {
                if (value is null)
                {
                    return "null";
                }
                else if (value is string)
                {
                    return $"\"{value}\"";
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
                                .Select(ToArgumentName);
                            return $"{genericTypeName}<{string.Join(",", genericArgs)}>";
                        }
                    }
                    catch { }
                    return type.Name;
                }
                else if (value is ParameterInfo parameter)
                {
                    var parameterType = parameter.ParameterType;                    
                    return ToArgumentName(parameterType);
                }
                return $"{value}";
            }
            return $"{method.Name}({string.Join(",", objects.Select(ToArgumentName))})";
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
    }
}
