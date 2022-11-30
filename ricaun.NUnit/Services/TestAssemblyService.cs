using NUnit.Framework;
using NUnit.Framework.Internal;
using ricaun.NUnit.Extensions;
using ricaun.NUnit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// TestAssemblyService
    /// </summary>
    public class TestAssemblyService : TestAttributeService
    {
        private readonly Assembly assembly;
        private readonly object[] parameters;

        /// <summary>
        /// Assembly
        /// </summary>
        public Assembly Assembly => this.assembly;
        /// <summary>
        /// Location
        /// </summary>
        public string Location => this.assembly?.Location;

        /// <summary>
        /// TestAssemblyService
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="parameters"></param>
        public TestAssemblyService(Assembly assembly, params object[] parameters)
        {
            this.assembly = assembly;
            this.parameters = parameters;
            this.assembly.LoadReferencedAssemblies();
        }

        /// <summary>
        /// TestAssemblyService
        /// </summary>
        /// <param name="location"></param>
        /// <param name="parameters"></param>
        public TestAssemblyService(string location, params object[] parameters) :
            this(Assembly.LoadFile(location), parameters)
        {

        }

        /// <summary>
        /// Get Test Types
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Type> GetTestTypes()
        {
            var types = new List<Type>();

            foreach (Type type in this.assembly.GetTypes())
            {
                if (!type.IsClass) continue;
                if (type.IsAbstract) continue;

                if (type.GetMethods().Where(e => TestEngineFilter.HasName(e.Name)).Any(this.AnyTestAttribute))
                {
                    types.Add(type);
                }
            }
            return types.OrderBy(e => e.Name);
        }

        /// <summary>
        /// GetTestTypeMethods
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MethodInfo> GetTestTypeMethods()
        {
            return GetTestTypes().SelectMany(e => e.GetMethods().Where(AnyTestAttribute)).OrderBy(e => GetMethodFullName(e));
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
            var names = new List<string>();
            if (TryGetAttributes<TestCaseAttribute>(method, out IEnumerable<TestCaseAttribute> testCases))
            {
                string GetTestCaseName(TestCaseAttribute testCaseAttribute)
                {
                    var name = testCaseAttribute.TestName ?? $"{method.Name}({string.Join(",", testCaseAttribute.Arguments)})";
                    return name;
                }
                return testCases.Select(GetTestCaseName).OrderBy(e => e).ToArray();
            }
            return new[] { method.Name };
        }

        /// <summary>
        /// GetTestTypeMethods
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public MethodInfo GetTestTypeMethods(string fullName)
        {
            return GetTestTypeMethods().FirstOrDefault(e => GetMethodFullName(e) == fullName);
        }

        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TestTypeModel> Test()
        {
            var result = new List<TestTypeModel>();
            var types = GetTestTypes();

            foreach (var type in types)
            {
                using (var console = new ConsoleWriterDateTime())
                {
                    var testTypeModel = new TestTypeModel();
                    try
                    {
                        using (var test = new TestService(type, parameters))
                        {
                            testTypeModel = test.Test();
                        }
                    }
                    catch (Exception ex)
                    {
                        testTypeModel = testTypeModel ?? new TestTypeModel();
                        testTypeModel.Name = type.Name;
                        testTypeModel.Message = testTypeModel.Message + Environment.NewLine + ex.ToString();
                        testTypeModel.Success = false;
                    }
                    finally
                    {
                        result.Add(testTypeModel);
                    }
                    testTypeModel.Console = console.GetString();
                    testTypeModel.Time = console.GetMillis();
                }
            }

            return result.ToArray();
        }

    }
}
