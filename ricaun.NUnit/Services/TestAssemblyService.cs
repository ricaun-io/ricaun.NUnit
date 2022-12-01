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

                //if (type.GetMethods().Where(AnyTestAttribute).Where(e => TestEngineFilter.HasName(e.Name)).Any())
                if (GetFilterTestMethods(type).Any())
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
        /// GetTestFullNames
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetTestFullNames()
        {
            return GetTestTypeMethods().SelectMany(e => GetTestAttributes(e).Select(a => GetTestFullName(e, a))).OrderBy(e => e);
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
