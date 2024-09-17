using ricaun.NUnit.Extensions;
using ricaun.NUnit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// TestAssemblyService
    /// </summary>
    internal class TestAssemblyService : TestAttributeService
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
        /// <param name="useExported">A flag indicating whether to use exported types only.</param>
        /// <returns></returns>
        private IEnumerable<Type> GetTestTypes(bool useExported = false)
        {
            var types = new List<Type>();

            foreach (Type type in GetAssemblyTypes(useExported))
            {
                if (!type.IsClass) continue;
                if (type.IsAbstract) continue;

                if (AnyMethodWithTestAttribute(type))
                {
                    types.Add(type);
                }
            }
            return types.OrderBy(e => e.FullName);
        }

        /// <summary>
        /// Get the types from the specified assembly.
        /// </summary>
        /// <param name="useExported">A flag indicating whether to use exported types only.</param>
        /// <returns>An array of types from the assembly.</returns>
        private Type[] GetAssemblyTypes(bool useExported)
        {
            return useExported ? this.assembly.GetExportedTypes() : this.assembly.GetTypes();
        }

        /// <summary>
        /// GetTestDictionaryTypeMethods
        /// </summary>
        /// <param name="useExported">A flag indicating whether to use exported types only.</param>
        /// <returns></returns>
        public IReadOnlyDictionary<Type, MethodInfo[]> GetTestDictionaryTypeMethods(bool useExported = false)
        {
            var result = new Dictionary<Type, MethodInfo[]>();
            var types = GetTestTypes(useExported);
            foreach (var type in types)
            {
                var methods = GetMethodWithTestAttribute(type).ToArray();
                if (methods.Any())
                {
                    result.Add(type, methods);
                }
            }
            return result;
        }

        /// <summary>
        /// GetTestFullNames
        /// </summary>
        /// <param name="useExported">A flag indicating whether to use exported types only.</param>
        /// <returns></returns>
        public IEnumerable<string> GetTestFullNames(bool useExported = false)
        {
            foreach (var typeMethod in GetTestDictionaryTypeMethods(useExported))
            {
                Type type = typeMethod.Key;
                MethodInfo[] methods = typeMethod.Value;
                foreach (MethodInfo method in methods)
                {
                    foreach (var attribute in GetTestAttributes(method))
                    {
                        yield return GetTestFullName(type, method, attribute);
                    }
                }
            }
        }

        /// <summary>
        /// RunTests
        /// </summary>
        /// <param name="useExported">A flag indicating whether to use exported types only.</param>
        /// <returns></returns>
        public IEnumerable<TestTypeModel> RunTests(bool useExported = false)
        {
            var result = new List<TestTypeModel>();
            var types = GetTestTypes(useExported).Where(AnyMethodWithTestAttributeAndFilter);

            foreach (var type in types)
            {
                using (var console = new ConsoleWriterDateTime())
                {
                    var testTypeModel = new TestTypeModel();
                    try
                    {
                        using (var test = new TestService(type, parameters))
                        {
                            testTypeModel = test.TestInstance();
                        }
                    }
                    catch (Exception ex)
                    {
                        testTypeModel.Message = testTypeModel.Message + Environment.NewLine + ex.ToString();
                        testTypeModel.Success = false;
                        foreach (var test in testTypeModel.Tests)
                        {
                            test.Message = test.Message + Environment.NewLine + ex.ToString();
                            test.Success = false;
                        }
                    }

                    result.Add(testTypeModel);
                    testTypeModel.Console = console.GetString();
                    testTypeModel.Time = console.GetMillis();
                }
            }

            return result.ToArray();
        }

    }
}
