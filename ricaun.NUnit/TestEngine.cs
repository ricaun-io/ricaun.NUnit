using NUnit.Framework;
using ricaun.NUnit.Extensions;
using ricaun.NUnit.Models;
using ricaun.NUnit.Services;
using System;
using System.Diagnostics;
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
        #region Initialize
        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>Return true if successful</returns>
        public static bool Initialize() => Initialize(out string message);

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>Return true if successful</returns>
        public static bool Initialize(out string message)
        {
            try
            {
                message = $"ricaun.NUnit {TestEngine.Version.ToString(3)} {TestEngine.VersionNUnit.ToString(3)}";
                return true;
            }
            catch (Exception ex)
            {
                message = $"{ex}";
            }
            return false;
        }
        #endregion

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

                        ValidateTestAssemblyNUnitVersion(testAssembly.Assembly);

                        var tests = testAssembly.Test();

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
        /// GetTestFullNames
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <returns></returns>
        public static string[] GetTestFullNames(string assemblyFile)
        {
            using (new AssemblyResolveService(Path.GetDirectoryName(assemblyFile)))
            {
                try
                {
                    var testAssembly = new TestAssemblyService(assemblyFile);
                    return testAssembly.GetTestFullNames().ToArray();
                }
                catch { }
            }
            return new string[] { };
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
            var assembly = ReflectionOnlyLoadFrom(assemblyFile);

            if (assembly is null) return null;
            AssemblyName nunitReference = GetNUnitReference(assembly);

            return nunitReference;
        }

        private static AssemblyName GetNUnitReference(Assembly assembly)
        {
            return assembly.GetReferencedAssemblies()
                .FirstOrDefault(e => e.Name.Equals(NUNIT_ASSEMBLY));
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

        private static void ValidateTestAssemblyNUnitVersion(Assembly assembly)
        {
            var nunitReference = GetNUnitReference(assembly);
            if (nunitReference is null)
            {
                return;
            }

            if (nunitReference.Version != VersionNUnit)
            {
                var message = $"'{NUNIT_ASSEMBLY}' version {nunitReference.Version} is not allow. Use the version {VersionNUnit}.";
                Debug.WriteLine(message);
                throw new FileLoadException(message);
            }
        }

        #endregion
    }
}
