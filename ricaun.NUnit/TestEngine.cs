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
    public static class TestEngine
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
                    using (new AssemblyResolveService(Path.GetDirectoryName(location)))
                    {
                        ValidateTestAssemblyNUnitVersion(location);

                        testAssemblyModel.FileName = Path.GetFileName(location);
                        var testAssembly = new TestAssemblyService(location, parameters);

                        testAssemblyModel.Name = testAssembly.Assembly.GetName().Name;
                        testAssemblyModel.Version = testAssembly.Assembly.GetName().Version.ToString(3);

                        if (testAssemblyModel.Name.EndsWith(testAssemblyModel.Version))
                        {
                            testAssemblyModel.Name = testAssemblyModel.Name
                                .Substring(0, testAssemblyModel.Name.Length - testAssemblyModel.Version.Length).Trim('.');
                        }

                        var task = Task.Run(async () =>
                        {
                            return await testAssembly.Test();
                        });
                        var tests = task.GetAwaiter().GetResult();

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

        /// <summary>
        /// Version of <see cref="NUnitAttribute"/>
        /// </summary>
        public static Version Version => typeof(NUnitAttribute).Assembly.GetName().Version;

        /// <summary>
        /// Check <paramref name="location"/> assembly contain NUnit reference
        /// </summary>
        /// <remarks>
        /// This method use <seealso cref="Assembly.ReflectionOnlyLoadFrom"/> 
        /// </remarks>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool ContainNUnit(string location)
        {
            return GetReferencedAssemblyNUnit(location) is not null;
        }

        #region nunit.framework
        private const string NUNIT_ASSEMBLY = "nunit.framework";
        /// <summary>
        /// Get Referenced NUnit using <seealso cref="Assembly.ReflectionOnlyLoadFrom"/>
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private static AssemblyName GetReferencedAssemblyNUnit(string location)
        {
            string reference = NUNIT_ASSEMBLY;
            var assembly = Assembly.ReflectionOnlyLoadFrom(location);

            var nunitReference = assembly.GetReferencedAssemblies()
                .FirstOrDefault(e => e.Name.Equals(reference));

            return nunitReference;
        }

        private static void ValidateTestAssemblyNUnitVersion(string location)
        {
            string reference = NUNIT_ASSEMBLY;

            var nunitReference = GetReferencedAssemblyNUnit(location);

            if (nunitReference is null)
            {
                return;
            }

            if (nunitReference.Version != Version)
            {
                var fileReference = Directory.GetFiles(Path.GetDirectoryName(location), $"{reference}.dll")
                    .FirstOrDefault();

                if (fileReference is not null)
                {
                    if (Assembly.ReflectionOnlyLoadFrom(fileReference).GetName().Version == nunitReference.Version)
                    {
                        var assemblyLoad = Assembly.LoadFile(fileReference);
                        // Console.WriteLine($"Assembly.LoadFile({assemblyLoad})");
                        return;
                    }
                }

                throw new FileLoadException($"'{reference}' version {nunitReference.Version} is not allow. Use the version {Version}.");
            }
        }
        #endregion
    }
}
