﻿using ricaun.NUnit.Extensions;
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
        /// <returns></returns>
        private IEnumerable<Type> GetTestTypes()
        {
            var types = new List<Type>();

            foreach (Type type in this.assembly.GetTypes())
            {
                if (!type.IsClass) continue;
                if (type.IsAbstract) continue;

                if (GetFilterTestMethods(type).Any())
                {
                    types.Add(type);
                }
            }
            return types.OrderBy(e => e.FullName);
        }

        /// <summary>
        /// GetTestTypeMethods
        /// </summary>
        /// <returns></returns>
        [Obsolete("Does not work with Abstract")]
        public IEnumerable<MethodInfo> GetTestTypeMethods()
        {
            return GetTestTypes().SelectMany(e => e.GetMethods().Where(AnyTestAttribute)).OrderBy(e => GetMethodFullName(e));
        }

        /// <summary>
        /// GetTestDictionaryTypeMethods
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<Type, MethodInfo[]> GetTestDictionaryTypeMethods()
        {
            var result = new Dictionary<Type, MethodInfo[]>();
            var types = GetTestTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods().Where(AnyTestAttribute).OrderBy(e => e.Name).ToArray();
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
        /// <returns></returns>
        public IEnumerable<string> GetTestFullNames()
        {
            foreach (var typeMethod in GetTestDictionaryTypeMethods())
            {
                Type type = typeMethod.Key;
                MethodInfo[] methods = typeMethod.Value;
                foreach (MethodInfo method in methods)
                {
                    foreach (var nUnitAttribute in GetTestAttributes(method))
                    {
                        yield return GetTestFullName(type, method, nUnitAttribute);
                    }
                }
            }
        }

        ///// <summary>
        ///// GetTestTypeMethods
        ///// </summary>
        ///// <param name="fullName"></param>
        ///// <returns></returns>
        //public MethodInfo GetTestTypeMethods(string fullName)
        //{
        //    return GetTestTypeMethods().FirstOrDefault(e => GetMethodFullName(e) == fullName);
        //}

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
                    //try
                    //{
                    //    using (var test = new TestService(type, parameters))
                    //    {
                    //        testTypeModel = test.Test();
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    testTypeModel = testTypeModel ?? new TestTypeModel();
                    //    testTypeModel.Name = type.Name;
                    //    testTypeModel.Message = testTypeModel.Message + Environment.NewLine + ex.ToString();
                    //    testTypeModel.Success = false;
                    //}
                    //finally
                    //{
                    //    result.Add(testTypeModel);
                    //}
                    //testTypeModel.Console = console.GetString();
                    //testTypeModel.Time = console.GetMillis();
                }
            }

            return result.ToArray();
        }

    }
}
