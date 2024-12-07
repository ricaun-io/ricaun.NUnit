using NUnit.Framework;
using ricaun.NUnit.Extensions;
using ricaun.NUnit.Models;
using ricaun.NUnit.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ricaun.NUnit
{
    /// <summary>
    /// TestEngine
    /// </summary>
    public static partial class TestEngine
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
        /// <param name="message">The initialization message</param>
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
        /// <returns>The TestAssemblyModel object</returns>
        public static TestAssemblyModel TestAssembly(string assemblyFile, params object[] parameters)
        {
            Exceptions.Clear();
            var testAssemblyModel = new TestAssemblyModel();
            using (var console = new ConsoleWriterDateTime())
            {
                try
                {
                    using (new AssemblyResolveCurrentDirectoryService(Path.GetDirectoryName(assemblyFile)))
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

                        try
                        {
                            testAssemblyModel.Tests.AddRange(testAssembly.RunTests());
                        }
                        catch (Exception ex)
                        {
                            Exceptions.Add(ex);
                            Debug.WriteLine(ex);

                            try
                            {
                                testAssemblyModel.Tests.AddRange(testAssembly.RunTests(true));
                            }
                            catch (Exception ex2)
                            {
                                Exceptions.Add(ex2);
                                Debug.WriteLine(ex2);
                            }
                        }

                        testAssemblyModel.Success = !testAssemblyModel.Tests.Any(e => !e.Success);
                    }
                }
                catch (Exception ex)
                {
                    Exceptions.Add(ex);
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
        /// Exceptions for the GetTestFullNames
        /// </summary>
        public static IList<Exception> Exceptions { get; } = new List<Exception>();

        /// <summary>
        /// GetTestFullNames
        /// </summary>
        /// <param name="assemblyFile">The assembly location</param>
        /// <returns>An array of test full names</returns>
        public static string[] GetTestFullNames(string assemblyFile)
        {
            Exceptions.Clear();
            using (new AssemblyResolveCurrentDirectoryService(Path.GetDirectoryName(assemblyFile)))
            {
                try
                {
                    var testAssembly = new TestAssemblyService(assemblyFile);
                    try
                    {
                        return testAssembly.GetTestFullNames().ToArray();
                    }
                    catch (Exception ex)
                    {
                        Exceptions.Add(ex);
                        Debug.WriteLine(ex);
                    }

                    // Retry with exported types only
                    return testAssembly.GetTestFullNames(true).ToArray();
                }
                catch (Exception ex)
                {
                    Exceptions.Add(ex);
                    Debug.WriteLine(ex);
                }
            }
            return new string[] { };
        }

        /// <summary>
        /// GetTestFullNames
        /// </summary>
        /// <param name="assemblyFile">The assembly location</param>
        /// <param name="directoryResolver">The directory resolver</param>
        /// <param name="includeSubDirectories">Flag to include subdirectories</param>
        /// <returns>An array of test full names</returns>
        public static string[] GetTestFullNames(string assemblyFile, string directoryResolver, bool includeSubDirectories = false)
        {
            using (new AssemblyResolveService(directoryResolver, includeSubDirectories))
            {
                return GetTestFullNames(assemblyFile);
            }
        }

        /// <summary>
        /// Get the metadata attributes of an assembly
        /// </summary>
        /// <param name="assemblyFile">The assembly location</param>
        /// <returns>An array of AssemblyMetadataAttribute</returns>
        public static AssemblyMetadataAttribute[] GetAssemblyMetadataAttributes(string assemblyFile)
        {
            return ReferenceLoaderUtils.GetAssemblyMetadataAttributes(assemblyFile);
        }

        /// <summary>
        /// Get the full name of a test
        /// </summary>
        /// <param name="testType">The test type model</param>
        /// <param name="test">The test model</param>
        /// <returns>The full name of the test</returns>
        [Obsolete("This method is obsolete, use TestModel.FullName")]
        internal static string GetTestFullName(TestTypeModel testType, TestModel test)
        {
            return $"{testType.Name}.{test.Name}.{test.Alias}";
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
        /// Check if the assembly contains a reference to NUnit
        /// </summary>
        /// <param name="assemblyFile">The assembly location</param>
        /// <returns>True if the assembly contains a reference to NUnit, otherwise false</returns>
        public static bool ContainNUnit(string assemblyFile)
        {
            return GetReferencedAssemblyNUnit(assemblyFile) is not null;
        }

        /// <summary>
        /// Check if the assembly contains a reference to NUnit
        /// </summary>
        /// <param name="assemblyFile">The assembly location</param>
        /// <param name="assemblyName">The assembly name</param>
        /// <returns>True if the assembly contains a reference to NUnit, otherwise false</returns>
        public static bool ContainNUnit(string assemblyFile, out AssemblyName assemblyName)
        {
            assemblyName = GetReferencedAssemblyNUnit(assemblyFile);
            return assemblyName is not null;
        }
        #endregion

        #region nunit.framework
        private const string NUNIT_ASSEMBLY = "nunit.framework";
        /// <summary>
        /// Get the referenced NUnit assembly using Assembly.ReflectionOnlyLoadFrom
        /// </summary>
        /// <param name="assemblyFile">The assembly location</param>
        /// <returns>The referenced NUnit assembly</returns>
        private static AssemblyName GetReferencedAssemblyNUnit(string assemblyFile)
        {
            var references = ReferenceLoaderUtils.GetReferencedAssemblies(assemblyFile);

            var nunitReference = references.FirstOrDefault(e => e.Name.Equals(NUNIT_ASSEMBLY));

            return nunitReference;
        }

        private static AssemblyName GetNUnitReference(Assembly assembly)
        {
            return assembly.GetReferencedAssemblies()
                .FirstOrDefault(e => e.Name.Equals(NUNIT_ASSEMBLY));
        }

        [Obsolete]
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
                var message = $"'{NUNIT_ASSEMBLY}' version {nunitReference.Version.ToString(3)} is not allowed. Use the version {VersionNUnit.ToString(3)}.";
                Debug.WriteLine(message);
                throw new FileLoadException(message);
            }
        }

        #endregion
    }
}
