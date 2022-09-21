using NUnit.Framework;
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
    public class TestAssemblyService : AttributeService
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

        private IEnumerable<Type> GetTestTypes()
        {
            var types = new List<Type>();

            foreach (Type type in this.assembly.GetTypes())
            {
                if (!type.IsClass) continue;
                if (type.IsAbstract) continue;

                if (type.GetMethods().Any(AnyAttributeName<TestAttribute>))
                {
                    types.Add(type);
                }
            }
            return types;
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
