﻿using NUnit.Framework;
using ricaun.NUnit.Extensions;
using ricaun.NUnit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// TestService
    /// </summary>
    internal class TestService : ActivatorService, IDisposable
    {
        private readonly TypeInstance typeInstance;
        private readonly Type type;
        private readonly object[] parameters;
        private object instance;

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
            this.typeInstance = new TypeInstance(type);
        }

        public TestService(TypeInstance typeInstance, params object[] parameters)
        {
            this.type = typeInstance.Type;
            this.parameters = parameters;
            this.typeInstance = typeInstance;
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
        /// Test Instance
        /// </summary>
        /// <returns></returns>
        public TestTypeModel TestInstance()
        {
            TestExecutionContextUtils.Clear();

            var methods = type.GetMethods();
            var testType = NewTestTypeModel();

            var testMethods = GetMethodWithTestAttributeAndFilter(typeInstance);

            if (IgnoreTestWithAttributes(type, out string ignoreMessage))
            {
                testType.Message = ignoreMessage;
                testType.Skipped = true;
                AddDefaultTestModels(testType, testMethods);
                return testType;
            }

            try
            {
                var instanceParameters = this.typeInstance.Parameters;
                if (instanceParameters.Length == 0)
                    instanceParameters = this.parameters;

                this.instance = CreateInstance(this.type, instanceParameters);
            }
            catch (Exception ex)
            {
                testType.Message = testType.Message + Environment.NewLine + ex.ToString();
                testType.Success = false;
                AddDefaultTestModels(testType, testMethods);
                return testType;
            }

            {
                bool OrderByDeclaringType(MethodInfo method)
                {
                    return method.DeclaringType == type;
                }

                var methodOneTimeSetUps = methods.Where(AnyAttributeName<OneTimeSetUpAttribute>)
                    .OrderBy(OrderByDeclaringType);
                var methodOneTimeTearDowns = methods.Where(AnyAttributeName<OneTimeTearDownAttribute>)
                    .OrderBy(OrderByDeclaringType)
                    .Reverse();

                var methodSetUps = methods.Where(AnyAttributeName<SetUpAttribute>)
                    .OrderBy(OrderByDeclaringType);
                var methodTearDowns = methods.Where(AnyAttributeName<TearDownAttribute>)
                    .OrderBy(OrderByDeclaringType)
                    .Reverse();

                var upOneResult = InvokeResultInstances(methodOneTimeSetUps);
                if (upOneResult.IsValid())
                {
                    foreach (var testMethod in testMethods)
                    {
                        foreach (var nUnitAttribute in GetTestAttributes(testMethod))
                        {
                            if (!HasFilterTestMethod(typeInstance.FullName, testMethod, nUnitAttribute)) continue;
                            var testModel = InvokeTestInstance(testType, testMethod, methodSetUps, methodTearDowns, nUnitAttribute);

                            //var testModel = InvokeTestInstance(testMethod, methodSetUp, methodTearDown, nUnitAttribute);
                            testType.Tests.Add(testModel);
                            TestEngine.InvokeResult(testModel);
                        }
                    }
                }
                var downOneResult = InvokeResultInstances(methodOneTimeTearDowns);

                var success = upOneResult.Success & downOneResult.Success;
                var skipped = upOneResult.Skipped & downOneResult.Skipped;
                var message = string.Join(Environment.NewLine, upOneResult.Message, downOneResult.Message).Trim();

                testType.Success = success & !testType.Tests.Any(e => e.Success == false);
                testType.Skipped = skipped & !testType.Tests.Any(e => e.Skipped == false);
                testType.Message = message;

                if (upOneResult.Success == false)
                {
                    AddDefaultTestModels(testType, testMethods);
                }
                if (downOneResult.Success == false)
                {
                    foreach (var testModel in testType.Tests)
                    {
                        testModel.Message += testType.Message;
                        testModel.Success = testType.Success;
                        testModel.Skipped = testType.Skipped;
                    }
                }

            }

            return testType;
        }

        /// <summary>
        /// NewTestTypeModel
        /// </summary>
        private TestTypeModel NewTestTypeModel()
        {
            var testType = new TestTypeModel();
            testType.Alias = typeInstance.FullName;
            testType.FullName = typeInstance.FullName;
            testType.Name = typeInstance.FullName;
            testType.Success = true;
            return testType;
        }

        private void AddDefaultTestModels(TestTypeModel testType, IEnumerable<MethodInfo> testMethods)
        {
            foreach (var testMethod in testMethods)
            {
                foreach (var nUnitAttribute in GetTestAttributes(testMethod))
                {
                    if (!HasFilterTestMethod(typeInstance.FullName, testMethod, nUnitAttribute)) continue;
                    var testModel = NewTestModel(testType, testMethod, nUnitAttribute);
                    testModel.Message = testType.Message;
                    testModel.Success = testType.Success;
                    testModel.Skipped = testType.Skipped;
                    testType.Tests.Add(testModel);
                    TestEngine.InvokeResult(testModel);
                }
            }
        }

        #endregion

        #region private

        private bool IgnoreTestWithAttributes(MemberInfo memberInfo, out string ignoreMessage)
        {
            ignoreMessage = "";

            if (TestEngineFilter.ExplicitEnabled == false)
            {
                if (TryGetAttribute(memberInfo, out ExplicitAttribute explicitAttribute))
                {
                    ignoreMessage = $"{explicitAttribute.GetReason()}";
                    if (string.IsNullOrEmpty(ignoreMessage))
                        ignoreMessage = explicitAttribute.GetType().Name;
                    return true;
                }
            }

            if (TryGetAttribute(memberInfo, out IgnoreAttribute ignoreAttribute))
            {
                ignoreMessage = $"{ignoreAttribute.GetReason()}";
                return true;
            }

            return false;
        }

        /// <summary>
        /// NewTestModel with type to work with abstract tests
        /// </summary>
        /// <param name="testType"></param>
        /// <param name="method"></param>
        /// <param name="nUnitAttribute"></param>
        /// <returns></returns>
        private TestModel NewTestModel(TestTypeModel testType, MethodInfo method, NUnitAttribute nUnitAttribute)
        {
            var test = new TestModel();
            test.Name = method.Name;
            test.Alias = GetTestName(method, nUnitAttribute);
            test.FullName = GetTestFullName(testType.FullName, method, nUnitAttribute);
            test.Success = true;
            return test;
        }

        /// <summary>
        /// Invoke Test Instance and Add In <paramref name="testType"/>
        /// </summary>
        /// <param name="testType"></param>
        /// <param name="method"></param>
        /// <param name="methodSetUps"></param>
        /// <param name="methodTearDowns"></param>
        /// <param name="nUnitAttribute"></param>
        /// <returns></returns>
        private TestModel InvokeTestInstance(TestTypeModel testType, MethodInfo method, IEnumerable<MethodInfo> methodSetUps, IEnumerable<MethodInfo> methodTearDowns, NUnitAttribute nUnitAttribute)
        {
            var testModel = NewTestModel(testType, method, nUnitAttribute);

            using (var console = new ConsoleWriterDateTime())
            {
                if (IgnoreTestWithAttributes(method, out string messageIgnore))
                {
                    testModel.Message = messageIgnore;
                    testModel.Console = console.GetString();
                    testModel.Time = console.GetMillis();
                    testModel.Skipped = true;
                    return testModel;
                }

                var upResult = InvokeResultInstances(methodSetUps);
                var methodResult = upResult;
                if (methodResult.IsValid())
                {
                    if (nUnitAttribute is TestCaseAttribute testCaseAttribute)
                        methodResult += InvokeResultInstanceTestCase(method, testCaseAttribute);
                    else
                        methodResult += InvokeResultInstance(method);
                }
                var downResult = InvokeResultInstances(methodTearDowns);
                methodResult += downResult;

                //var success = upResult.Success & methodResult.Success & downResult.Success;
                //var message = string.Join(Environment.NewLine, upResult.Message, methodResult.Message, downResult.Message).Trim();

                testModel.Success = methodResult.Success;
                testModel.Skipped = methodResult.Skipped;
                testModel.Message = methodResult.Message;
                testModel.Console = console.GetString();
                testModel.Time = console.GetMillis();
            }

            return testModel;
        }

        #endregion

        #region InvokeResult
        private class InvokeResult
        {
            public bool Success { get; set; } = true;
            public bool Skipped { get; set; } = false;
            public string Message { get; set; }

            /// <summary>
            /// Is Success and Not Skipped
            /// </summary>
            /// <returns></returns>
            public bool IsValid()
            {
                return Success && !Skipped;
            }

            /// <summary>
            /// Join InvokeResult
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static InvokeResult operator +(InvokeResult left, InvokeResult right)
            {
                left.Success = left.Success & right.Success;
                left.Skipped = left.Skipped | right.Skipped;
                left.Message = string.Join(Environment.NewLine, left.Message, right.Message).Trim();
                return left;
            }
        }
        private InvokeResult InvokeResultInstances(IEnumerable<MethodInfo> methods)
        {
            var result = new InvokeResult();
            foreach (var method in methods)
            {
                result += InvokeResultInstance(method);
                if (result.IsValid() == false)
                    break;
            }
            return result;
        }
        private InvokeResult InvokeResultInstance(MethodInfo method)
        {
            TestExecutionContextUtils.Clear();
            var result = new InvokeResult();
            if (method is null)
                return result;

            if (IgnoreTestWithAttributes(method, out string message))
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
        private InvokeResult InvokeResultInstanceTestCase(MethodInfo method, TestCaseAttribute testCase)
        {
            TestExecutionContextUtils.Clear();
            var result = new InvokeResult();
            if (method is null)
                return result;

            if (IgnoreTestWithAttributes(method, out string message))
            {
                result.Message = message;
                result.Skipped = true;
                return result;
            }

            try
            {
                var value = Invoke(this.instance, method, testCase.Arguments);

                if (IsValueExpectedResult(testCase, value, out object expectedResult) == false)
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
        private bool IsValueExpectedResult(TestCaseAttribute testCase, object value, out object expectedResult)
        {
            expectedResult = null;
            if (testCase is null)
                return true;

            expectedResult = testCase.ExpectedResult ?? null;
            var equals = (value is not null) ? value.Equals(expectedResult) : expectedResult is null;
            return equals;
        }
        #endregion
    }
}
