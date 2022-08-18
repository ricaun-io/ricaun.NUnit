using ricaun.NUnit.Models;
using ricaun.NUnit.Services;
using System;
using System.IO;
using System.Linq;
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
                }
                catch (Exception ex)
                {
                    testAssemblyModel.Message = ex.Message;
                }
                testAssemblyModel.Console = console.GetString();
                testAssemblyModel.Time = console.GetMillis();
                testAssemblyModel.Success = !testAssemblyModel.Tests.Any(e => !e.Success);
            }
            return testAssemblyModel;
        }
    }
}
