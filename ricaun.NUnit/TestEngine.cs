using NUnit.Framework;
using ricaun.NUnit.Extensions;
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
    public static class TestEngine
    {
        /// <summary>
        /// Test Assembly
        /// </summary>
        /// <param name="assemblyFile">Assembly location</param>
        /// <param name="parameters">Parameters to create classes and methods</param>
        /// <returns></returns>
        public static TestAssemblyModel TestAssembly(string assemblyFile, params object[] parameters)
        {
            return TestAssemblyOptions(assemblyFile, false, parameters);
        }

        /// <summary>
        /// Test Assembly
        /// </summary>
        /// <param name="assemblyFile">Assembly location</param>
        /// <param name="onlyReadTest"></param>
        /// <param name="parameters">Parameters to create classes and methods</param>
        /// <returns></returns>
        public static TestAssemblyModel TestAssemblyOptions(string assemblyFile, bool onlyReadTest, params object[] parameters)
        {
            var testAssemblyModel = new TestAssemblyModel();
            using (var console = new ConsoleWriterDateTime())
            {
                try
                {
                    using (new AssemblyResolveService(Path.GetDirectoryName(assemblyFile)))
                    {
                        testAssemblyModel.FileName = Path.GetFileName(assemblyFile);
                        var testAssembly = new TestAssemblyService(assemblyFile, parameters);

                        testAssemblyModel.Name = testAssembly.Assembly.GetName().Name;
                        testAssemblyModel.Version = testAssembly.Assembly.GetName().Version.ToString(3);

                        if (testAssemblyModel.Name.EndsWith(testAssemblyModel.Version))
                        {
                            testAssemblyModel.Name = testAssemblyModel.Name
                                .Substring(0, testAssemblyModel.Name.Length - testAssemblyModel.Version.Length).Trim('.');
                        }

                        var tests = testAssembly.Test(onlyReadTest);

                        testAssemblyModel.Tests.AddRange(tests);
                        testAssemblyModel.Success = !testAssemblyModel.Tests.Any(e => !e.Success);
                    }

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

        #region NUnit
        /// <summary>
        /// Version of <see cref="TestEngine"/>
        /// </summary>
        public static Version Version => typeof(TestEngine).Assembly.GetName().Version;

        /// <summary>
        /// Version of <see cref="NUnitAttribute"/>
        /// </summary>
        public static Version VersionNUnit => typeof(NUnitAttribute).Assembly.GetName().Version;

        /// <summary>
        /// Check <paramref name="assemblyFile"/> assembly contain NUnit reference
        /// </summary>
        /// <remarks>
        /// This method use <seealso cref="Assembly.ReflectionOnlyLoadFrom"/> 
        /// </remarks>
        /// <param name="assemblyFile"></param>
        /// <returns></returns>
        public static bool ContainNUnit(string assemblyFile)
        {
            return GetReferencedAssemblyNUnit(assemblyFile) is not null;
        }
        #endregion

        #region nunit.framework
        private const string NUNIT_ASSEMBLY = "nunit.framework";
        /// <summary>
        /// Get Referenced NUnit using <seealso cref="Assembly.ReflectionOnlyLoadFrom"/>
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <returns></returns>
        private static AssemblyName GetReferencedAssemblyNUnit(string assemblyFile)
        {
            string reference = NUNIT_ASSEMBLY;
            var assembly = ReflectionOnlyLoadFrom(assemblyFile);

            if (assembly is null) return null;

            var nunitReference = assembly.GetReferencedAssemblies()
                .FirstOrDefault(e => e.Name.Equals(reference));

            return nunitReference;
        }

        private static Assembly ReflectionOnlyLoadFrom(string assemblyFile)
        {
            try
            {
                return Assembly.ReflectionOnlyLoadFrom(assemblyFile);
            }
            catch
            {
                var assemblyName = AssemblyName.GetAssemblyName(assemblyFile);
                return AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies()
                    .FirstOrDefault(e => e.GetName().ToString().Equals(assemblyName.ToString()));
            }
        }
        #endregion
    }
}
