using NUnit.Framework;
using ricaun.NUnit.Models;
using ricaun.NUnit.Services;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ricaun.NUnit
{
    /// <summary>
    /// TestEngine
    /// </summary>
    public class TestEngine
    {
        /// <summary>
        /// Test Assembly
        /// </summary>
        /// <param name="location">Assembly location</param>
        /// <param name="parameters">Parameters to create classes and methods</param>
        /// <returns></returns>
        public static TestAssemblyModel TestAssembly(string location, params object[] parameters)
        {
            var testAssemblyModel = new TestAssemblyModel();
            using (var console = new ConsoleWriterDateTime())
            {
                try
                {
                    ValidateTestAssemblyNUnitVersion(location);
                    //if (CanTestAssembly(location) == false)
                    //{
                    //    throw new Exception($"nunit.framework {Version}");
                    //}

                    testAssemblyModel.FileName = Path.GetFileName(location);
                    var testAssembly = new TestAssemblyService(location, parameters);

                    testAssemblyModel.Name = testAssembly.Assembly.GetName().Name;
                    testAssemblyModel.Version = testAssembly.Assembly.GetName().Version.ToString(3);
                    try
                    {
                        testAssemblyModel.Name = testAssembly.Assembly.GetTypes().FirstOrDefault().Namespace;
                    }
                    catch { }

                    var task = Task.Run(async () =>
                    {
                        return await testAssembly.Test();
                    });
                    var tests = task.GetAwaiter().GetResult();

                    testAssemblyModel.Tests.AddRange(tests);
                    testAssemblyModel.Success = !testAssemblyModel.Tests.Any(e => !e.Success);
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(testAssemblyModel.Name))
                        testAssemblyModel.Name = ex.GetType().Name;

                    testAssemblyModel.Success = false;
                    testAssemblyModel.Message = ex.ToString();
                }
                testAssemblyModel.Console = console.GetString();
                testAssemblyModel.Time = console.GetMillis();
            }
            return testAssemblyModel;
        }

        /// <summary>
        /// Version of <see cref="NUnitAttribute"/>
        /// </summary>
        public static Version Version => typeof(NUnitAttribute).Assembly.GetName().Version;

        private static void ValidateTestAssemblyNUnitVersion(string location)
        {
            string reference = "nunit.framework";
            var assembly = Assembly.ReflectionOnlyLoadFrom(location);
            var nunitReference = assembly.GetReferencedAssemblies()
                .FirstOrDefault(e => e.Name.Equals(reference));

            if (nunitReference is null)
                return;

            if (nunitReference.Version != Version)
            {
                throw new FileLoadException($"'{reference}' version {nunitReference.Version} is not allow. Use the version {Version}.");
            }
        }
    }
}
