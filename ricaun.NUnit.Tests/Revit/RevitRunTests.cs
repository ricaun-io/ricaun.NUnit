using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace ricaun.NUnit.Tests.Revit
{
    public class RevitRunTests
    {
        private string GetPathFile(string filePath)
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(location, filePath);
        }

        [TestCase("Revit\\RevitTest.dll")]
        [TestCase("Revit\\RevitTestDB.dll")]
        public void GetTests(string pathFile)
        {
            var names = TestEngine.GetTestFullNames(GetPathFile(pathFile));

            foreach (var name in names)
                Console.WriteLine($"{name}");

            foreach (var ex in TestEngine.Exceptions)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        [TestCase("Revit\\RevitTest.dll")]
        [TestCase("Revit\\RevitTestDB.dll")]
        public void RunTests(string pathFile)
        {
            var testAssemblyModel = TestEngine.TestAssembly(GetPathFile(pathFile));

            Console.WriteLine(testAssemblyModel.AsString());

            foreach (var ex in TestEngine.Exceptions)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
