﻿using NUnit.Framework;
using ricaun.NUnit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// TestService
    /// </summary>
    public class TestService : ActivatorService, IDisposable
    {
        private readonly Type type;
        private readonly object[] parameters;
        private object instance;

        /// <summary>
        /// IgnoreAttributes
        /// </summary>
        public Type[] IgnoreAttributes { get; set; } = new[] {
            typeof(IgnoreAttribute),
            typeof(TestCaseAttribute),
            typeof(ExplicitAttribute)
        };

        #region Constructor
        /// <summary>
        /// TestService
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        public TestService(Type type, params object[] parameters)
        {
            this.type = type;
            this.parameters = parameters;
            this.instance = CreateInstance(type, this.parameters);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            TryForceDispose(this.instance);
        }
        #endregion

        #region public

        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        public TestTypeModel Test()
        {
            var methods = type.GetMethods();
            var testType = new TestTypeModel();
            testType.Name = type.FullName;
            testType.Success = true;

            {
                if (IgnoreTest(type, out string ignoreMessage))
                {
                    testType.Message = ignoreMessage;
                    testType.Skipped = true;
                    return testType;
                }

                var testMethods = methods.Where(AnyAttributeName<TestAttribute>);

                //if (onlyReadTest)
                //{
                //    foreach (var testMethod in testMethods)
                //    {
                //        var testModel = new TestModel() { Name = testMethod.Name };
                //        testType.Tests.Add(testModel);
                //    }
                //    return testType;
                //}

                var methodOneTimeSetUp = methods.FirstOrDefault(AnyAttributeName<OneTimeSetUpAttribute>);
                var methodOneTimeTearDown = methods.FirstOrDefault(AnyAttributeName<OneTimeTearDownAttribute>);

                var methodSetUp = methods.FirstOrDefault(AnyAttributeName<SetUpAttribute>);
                var methodTearDown = methods.FirstOrDefault(AnyAttributeName<TearDownAttribute>);

                var upOneResult = InvokeResultInstance(methodOneTimeSetUp);
                if (upOneResult.Success)
                {
                    foreach (var testMethod in testMethods)
                    {
                        var testModel = InvokeTestInstance(testMethod, methodSetUp, methodTearDown);
                        testType.Tests.Add(testModel);
                    }
                }
                var downOneResult = InvokeResultInstance(methodOneTimeTearDown);

                var success = upOneResult.Success & downOneResult.Success;
                var skipped = upOneResult.Skipped & downOneResult.Skipped;
                var message = string.Join(Environment.NewLine, upOneResult.Message, downOneResult.Message).Trim();

                testType.Success = success & !testType.Tests.Any(e => e.Success == false);
                testType.Skipped = skipped & !testType.Tests.Any(e => e.Skipped == false);
                testType.Message = message;
            }

            return testType;
        }

        #endregion

        #region private

        private bool IgnoreTest(MemberInfo memberInfo, out string ignoreMessage)
        {
            ignoreMessage = "";
            foreach (var ignoreAttribute in IgnoreAttributes)
            {
                if (HasAttributeName(memberInfo, ignoreAttribute))
                {
                    ignoreMessage = $"IgnoreTest: '{memberInfo.Name}' => '{ignoreAttribute.Name}'";
                    return true;
                }
            }
            return false;
        }

        private TestModel InvokeTestInstance(MethodInfo method, MethodInfo methodSetUp, MethodInfo methodTearDown)
        {
            var test = new TestModel();
            test.Name = method.Name;
            test.Success = true;

            using (var console = new ConsoleWriterDateTime())
            {
                if (IgnoreTest(method, out string messageIgnore))
                {
                    test.Message = messageIgnore;
                    test.Console = console.GetString();
                    test.Time = console.GetMillis();
                    test.Skipped = true;
                    return test;
                }

                var upResult = InvokeResultInstance(methodSetUp);
                var methodResult = upResult;
                if (upResult.Success)
                {
                    methodResult = InvokeResultInstance(method);
                }
                var downResult = InvokeResultInstance(methodTearDown);

                var success = upResult.Success & methodResult.Success & downResult.Success;
                var message = string.Join(Environment.NewLine, upResult.Message, methodResult.Message, downResult.Message).Trim();

                test.Success = success;
                test.Skipped = methodResult.Skipped;
                test.Message = message;
                test.Console = console.GetString();
                test.Time = console.GetMillis();
            }

            return test;
        }

        #endregion

        #region InvokeResult

        private class InvokeResult
        {
            public bool Success { get; set; } = true;
            public bool Skipped { get; set; } = false;
            public string Message { get; set; }
        }
        private InvokeResult InvokeResultInstance(MethodInfo method)
        {
            var result = new InvokeResult();
            if (method is null)
                return result;

            if (IgnoreTest(method, out string message))
            {
                result.Message = message;
                result.Skipped = true;
                return result;
            }

            try
            {
                var value = Invoke(this.instance, method, this.parameters);

                if (IsValueExpectedResult(method, value, out object expectedResult) == false)
                {
                    result.Success = false;
                    result.Message = $"Expected:\t{expectedResult}\tBut was:\t{value}";
                }

            }
            catch (Exception ex)
            {
                var exInner = ex.InnerException is null ? ex : ex.InnerException;
                result.Success = false;
                result.Message = exInner.ToString();
                switch (exInner)
                {
                    case SuccessException success:
                        result.Success = true;
                        result.Message = success.Message;
                        break;
                    case IgnoreException ignore:
                        result.Success = true;
                        result.Skipped = true;
                        result.Message = ignore.Message;
                        break;
                    case AssertionException assertion:
                        result.Message = assertion.Message + Environment.NewLine + assertion.ToString();
                        break;
                }
            }
            return result;
        }

        private bool IsValueExpectedResult(MethodInfo method, object value, out object expectedResult)
        {
            expectedResult = null;
            var testAttribute = GetAttribute<TestAttribute>(method);

            if (testAttribute is null)
                return true;

            expectedResult = testAttribute.ExpectedResult ?? null;
            var equals = (value is not null) ? value.Equals(expectedResult) : expectedResult is null;
            return equals;
        }
        #endregion
    }
}
