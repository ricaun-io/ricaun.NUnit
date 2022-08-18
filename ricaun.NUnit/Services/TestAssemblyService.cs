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
        }

        /// <summary>
        /// TestAssemblyService
        /// </summary>
        /// <param name="location"></param>
        /// <param name="parameters"></param>
        public TestAssemblyService(string location, params object[] parameters) :
            this(Assembly.LoadFile(location), parameters)
        {
            //this.assembly = Assembly.ReflectionOnlyLoadFrom(location);
        }

        private string[] References = new[] { "nunit.framework", "RevitAPI" };

        private IEnumerable<Type> GetTestTypes()
        {
            var types = new List<Type>();

            foreach (Type type in this.assembly.GetTypes())
            {
                if (!type.IsClass) continue;
                if (type.IsAbstract) continue;

                if (type.GetMethods().Any(AnyAttribute<TestAttribute>))
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
        public async Task<IEnumerable<TestTypeModel>> Test()
        {
            var result = new List<TestTypeModel>();

            var types = GetTestTypes();

            foreach (var type in types)
            {
                var testModel = new TestTypeModel();
                try
                {
                    using (var test = new TestService(type, parameters))
                    {
                        testModel = await test.Test();
                    }
                }
                catch (Exception ex)
                {
                    testModel = testModel ?? new TestTypeModel();
                    testModel.Name = type.Name;
                    testModel.Message = testModel.Message + Environment.NewLine + ex.ToString();
                    testModel.Success = false;
                }
                finally
                {
                    result.Add(testModel);
                }
            }

            return result.ToArray();
        }

    }
}
