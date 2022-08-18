using NUnit.Framework;
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
        private List<TestModel> TestModels { get; } = new();
        private ConsoleWriterDateTime consoleWriter = new();

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
            consoleWriter.Dispose();
            TryForceDispose(this.instance);
        }
        #endregion

        #region public

        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        public async Task<TestTypeModel> Test()
        {
            var methods = type.GetMethods();
            var testType = new TestTypeModel();
            testType.Name = type.FullName;
            testType.Success = true;

            using (var console = new ConsoleWriterDateTime())
            {
                if (IgnoreTest(type, out string ignoreMessage))
                {
                    testType.Message = ignoreMessage;
                    testType.Console = console.GetString();
                    testType.Time = console.GetMillis();
                    return testType;
                }

                var methodOneTimeSetUp = methods.FirstOrDefault(AnyAttribute<OneTimeSetUpAttribute>);
                var methodOneTimeTearDown = methods.FirstOrDefault(AnyAttribute<OneTimeTearDownAttribute>);

                var methodSetUp = methods.FirstOrDefault(AnyAttribute<SetUpAttribute>);
                var methodTearDown = methods.FirstOrDefault(AnyAttribute<TearDownAttribute>);

                var testMethods = methods.Where(AnyAttribute<TestAttribute>);

                var upOneResult = await InvokeResultInstance(methodOneTimeSetUp);
                if (upOneResult.Success)
                {
                    foreach (var testMethod in testMethods)
                    {
                        var testModel = await InvokeTestInstance(testMethod, methodSetUp, methodTearDown);
                        testType.Tests.Add(testModel);
                    }
                }
                var downOneResult = await InvokeResultInstance(methodOneTimeTearDown);

                var success = upOneResult.Success & downOneResult.Success;
                var message = string.Join(Environment.NewLine, upOneResult.Message, downOneResult.Message).Trim();

                testType.Success = success & !testType.Tests.Any(e => e.Success == false);
                testType.Message = message;
                testType.Console = console.GetString();
                testType.Time = console.GetMillis();
            }

            return testType;
        }

        /*
        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TestModel>> Test()
        {
            var methods = type.GetMethods();

            if (IgnoreTest(type, out string ignoreMessage))
            {
                AddTestModel(type.Name, true, ignoreMessage);
                return Enumerable.Empty<TestModel>();
            }

            var methodOneTimeSetUp = methods.FirstOrDefault(AnyAttribute<OneTimeSetUpAttribute>);
            var methodOneTimeTearDown = methods.FirstOrDefault(AnyAttribute<OneTimeTearDownAttribute>);

            var methodSetUp = methods.FirstOrDefault(AnyAttribute<SetUpAttribute>);
            var methodTearDown = methods.FirstOrDefault(AnyAttribute<TearDownAttribute>);

            var testMethods = methods.Where(AnyAttribute<TestAttribute>);

            if (await InvokeInstance(methodOneTimeSetUp))
            {
                foreach (var testMethod in testMethods)
                {
                    await InvokeInstance(testMethod, methodSetUp, methodTearDown);
                }
            }
            await InvokeInstance(methodOneTimeTearDown);

            return TestModels;
        }
        */
        #endregion

        #region private
        private void AddTestModel(string name, bool success, string message = "")
        {
            var textModel = new TestModel()
            {
                Name = name,
                Success = success,
                Message = message,
                Console = consoleWriter.GetString(),
                Time = consoleWriter.GetMillis()
            };

            TestModels.Add(textModel);
        }
        private bool IgnoreTest(MemberInfo memberInfo, out string ignoreMessage)
        {
            ignoreMessage = "";
            foreach (var ignoreAttribute in IgnoreAttributes)
            {
                if (HasAttribute(memberInfo, ignoreAttribute))
                {
                    ignoreMessage = $"IgnoreTest: '{memberInfo.Name}' => '{ignoreAttribute.Name}'";
                    return true;
                }
            }
            return false;
        }

        /*
        private async Task<bool> InvokeInstance(MethodInfo method, MethodInfo methodSetUp = null, MethodInfo methodTearDown = null)
        {
            if (method is null)
                return true;

            if (IgnoreTest(method, out string messageIgnore))
            {
                AddTestModel(method.Name, true, messageIgnore);
                return true;
            }

            var upResult = await InvokeResultInstance(methodSetUp);
            var methodResult = upResult;
            if (upResult.Success)
            {
                methodResult = await InvokeResultInstance(method);
            }
            var downResult = await InvokeResultInstance(methodTearDown);

            var success = upResult.Success & methodResult.Success & downResult.Success;

            var message = string.Join(Environment.NewLine, upResult.Message, methodResult.Message, downResult.Message).Trim();

            //message += $"{upResult.Success} {methodResult.Success} {downResult.Success}";

            AddTestModel(method.Name, success, message);

            return success;
        }
        */
        private async Task<TestModel> InvokeTestInstance(MethodInfo method, MethodInfo methodSetUp, MethodInfo methodTearDown)
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
                    return test;
                }

                var upResult = await InvokeResultInstance(methodSetUp);
                var methodResult = upResult;
                if (upResult.Success)
                {
                    methodResult = await InvokeResultInstance(method);
                }
                var downResult = await InvokeResultInstance(methodTearDown);

                var success = upResult.Success & methodResult.Success & downResult.Success;
                var message = string.Join(Environment.NewLine, upResult.Message, methodResult.Message, downResult.Message).Trim();

                test.Success = success;
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
            public string Message { get; set; }
        }
        private async Task<InvokeResult> InvokeResultInstance(MethodInfo method)
        {
            var result = new InvokeResult();
            if (method is null)
                return result;

            if (IgnoreTest(method, out string message))
            {
                result.Message = message;
                return result;
            }

            try
            {
                await Invoke(this.instance, method, this.parameters);
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
                    case AssertionException assertion:
                        result.Message = assertion.Message + Environment.NewLine + assertion.ToString();
                        break;
                }
            }
            return result;
        }
        #endregion
    }
}
