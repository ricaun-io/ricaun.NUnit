﻿using NUnit.Framework;
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
            return type.GetMethods().Where(AnyTestAttribute).OrderBy(e => e.Name);
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

        ///// <summary>
        ///// GetFilterTestMethods
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //[Obsolete]
        //public IEnumerable<MethodInfo> GetFilterTestMethods(Type type)
        //{
        //    var methods = type.GetMethods().OrderBy(e => e.Name);
        //    var testMethods = methods.Where(AnyTestAttribute);
        //    var onlyTypeTests = testMethods.Where(m => GetTestAttributes(m).Any(a => HasFilterTestMethod(m, a)));
        //    //foreach (var testMethod in testMethods)
        //    //{
        //    //    //Debug.WriteLine($"{testMethod.Name} {testMethod.GetCustomAttributes(true).Count()}");
        //    //    Debug.WriteLine($"{testMethod.Name} {string.Join(" ", testMethod.GetCustomAttributes(true).Select(e => e.GetType().Name))}");
        //    //    Debug.WriteLine($"{testMethod.Name} {testMethod.GetCustomAttributes(true).OfType<TestAttribute>().Count()}");
        //    //}
        //    //Debug.WriteLine($"Debug:\t{type}:\t{string.Join(" ", onlyTypeTests.Select(e => e.Name))}");
        //    return onlyTypeTests;
        //}

        ///// <summary>
        ///// HasFilterTestMethod
        ///// </summary>
        ///// <param name="method"></param>
        ///// <param name="nUnitAttribute"></param>
        ///// <returns></returns>
        //public bool HasFilterTestMethod(MethodInfo method, NUnitAttribute nUnitAttribute)
        //{
        //    return TestEngineFilter.HasName(GetTestFullName(method, nUnitAttribute));
        //}
        //public bool HasFilterTestMethod(string typeFullName, MethodInfo method, NUnitAttribute nUnitAttribute)
        //{
        //    return TestEngineFilter.HasName(GetTestFullName(typeFullName, method, nUnitAttribute));
        //}

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

        ///// <summary>
        ///// GetMethodFullName
        ///// </summary>
        ///// <param name="method"></param>
        ///// <returns></returns>
        //[Obsolete]
        //public string GetMethodFullName(MethodInfo method)
        //{
        //    return GetMethodFullName(method.DeclaringType, method);
        //}

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

        ///// <summary>
        ///// GetTestFullName
        ///// </summary>
        ///// <param name="method"></param>
        ///// <param name="nUnitAttribute"></param>
        ///// <returns></returns>
        //[Obsolete]
        //public string GetTestFullName(MethodInfo method, NUnitAttribute nUnitAttribute)
        //{
        //    return GetMethodFullName(method) + "." + GetTestName(method, nUnitAttribute);
        //}

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
            if (attribute is TestCaseAttribute testCaseAttribute)
            {
                return testCaseAttribute.TestName ?? GetTestNameWithArguments(method, testCaseAttribute.Arguments);
            }

            var parameters = method.GetParameters();
            if (parameters.Any())
            {
                return GetTestNameWithArguments(method, parameters);
            }

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
                else if (value is ParameterInfo parameter)
                {
                    return parameter.ParameterType.Name;
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
