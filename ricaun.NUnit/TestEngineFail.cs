using ricaun.NUnit.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace ricaun.NUnit
{
    /// <summary>
    /// TestEngine
    /// </summary>
    public static partial class TestEngine
    {
        /// <summary>
        /// Create <see cref="TestAssemblyModel"/> with <paramref name="exception"/> fail in each <paramref name="testNames"/>.
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="exception"></param>
        /// <param name="testNames"></param>
        /// <returns></returns>
        /// <remarks>If <paramref name="testNames"/> is empty, the <seealso cref="TestEngine.GetTestFullNames(string)"/> is used.</remarks>
        public static TestAssemblyModel Fail(
            string assemblyFile,
            Exception exception,
            params string[] testNames)
        {
            var testTypeModel = new TestTypeModel();
            testTypeModel.Tests = new List<TestModel>();

            var testAssemblyModel = new TestAssemblyModel();
            testAssemblyModel.Name = Path.GetFileName(assemblyFile);
            testAssemblyModel.FileName = Path.GetFileName(assemblyFile);
            testAssemblyModel.Tests = new List<TestTypeModel>() { testTypeModel };

            if (testNames.Length == 0)
                testNames = TestEngine.GetTestFullNames(assemblyFile);

            foreach (var testName in testNames)
            {
                var testModel = new TestModel()
                {
                    Name = testName,
                    FullName = testName,
                    Success = false,
                    Message = exception.Message,
                };

                testTypeModel.Tests.Add(testModel);
            }

            return testAssemblyModel;
        }
    }
}
