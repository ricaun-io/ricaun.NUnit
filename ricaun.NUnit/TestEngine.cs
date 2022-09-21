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
            var testAssemblyModel = new TestAssemblyModel();
            using (var console = new ConsoleWriterDateTime())
            {
                try
                {
                    //using (AppDomain.CurrentDomain.GetAssemblyResolveDisposable())
                    {
                        using (new AssemblyResolveService(Path.GetDirectoryName(assemblyFile)))
                        {
                            //ValidateTestAssemblyNUnitVersion(assemblyFile);

                            testAssemblyModel.FileName = Path.GetFileName(assemblyFile);
                            var testAssembly = new TestAssemblyService(assemblyFile, parameters);

                            testAssemblyModel.Name = testAssembly.Assembly.GetName().Name;
                            testAssemblyModel.Version = testAssembly.Assembly.GetName().Version.ToString(3);

                            if (testAssemblyModel.Name.EndsWith(testAssemblyModel.Version))
                            {
                                testAssemblyModel.Name = testAssemblyModel.Name
                                    .Substring(0, testAssemblyModel.Name.Length - testAssemblyModel.Version.Length).Trim('.');
                            }

                            var tests = testAssembly.Test();

                            testAssemblyModel.Tests.AddRange(tests);
                            testAssemblyModel.Success = !testAssemblyModel.Tests.Any(e => !e.Success);
                        }

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

        private static void ValidateTestAssemblyNUnitVersion(string assemblyFile)
        {
            string reference = NUNIT_ASSEMBLY;

            var nunitReference = GetReferencedAssemblyNUnit(assemblyFile);

            if (nunitReference is null)
            {
                return;
            }

            if (nunitReference.Version != Version)
            {
                var fileReference = Directory.GetFiles(Path.GetDirectoryName(assemblyFile), $"{reference}.dll")
                    .FirstOrDefault();

                if (fileReference is not null)
                {
                    if (ReflectionOnlyLoadFrom(fileReference).GetName().Version == nunitReference.Version)
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
