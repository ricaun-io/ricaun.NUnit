using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ricaun.NUnit.Tests.Revit
{
    [Explicit]
    public class RevitTests
    {
        #region private
        private string GetResolverDirectory(int version)
        {
            return $"C:\\Program Files\\Autodesk\\Revit {version}";
        }
        private string GetPathFile(int version)
        {
            return GetPathFile($"Revit\\{version}\\RevitTest1.dll");
        }
        private string GetPathFile(string filePath)
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(location, filePath);
        }
        #endregion

        [Test]
        public void TestAssembly_ContainNUnit()
        {
            var pathFile = GetPathFile("Revit\\RevitTest.dll");
            Assert.IsTrue(TestEngine.ContainNUnit(pathFile));
        }

        [Test]
        public void TestAssembly_Names()
        {
            var pathFile = GetPathFile("Revit\\RevitTest.dll");
            var names = TestEngine.GetTestFullNames(pathFile);

            foreach (var name in names)
                Console.WriteLine($"{name}");

            Assert.AreEqual(4, names.Length);
            Assert.IsTrue(names.Any(e => e.Contains("(*)")));
        }

        [Test]
        public void TestAssembly_Names_DB()
        {
            var pathFile = GetPathFile("Revit\\RevitTestDB.dll");
            var names = TestEngine.GetTestFullNames(pathFile);

            foreach (var name in names)
                Console.WriteLine($"{name}");

            Assert.AreEqual(2, names.Length);
            Assert.IsTrue(names.Any(e => e.Contains("(*)")));
        }


        [TestCase(2021)]
        [TestCase(2022)]
        [TestCase(2023)]
        [TestCase(2024)]
        public void TestAssembly_Names_Resolver(int version)
        {
            var pathFile = GetPathFile("Revit\\RevitTest.dll");
            var directory = GetResolverDirectory(version);
            if (!Directory.Exists(directory)) Assert.Ignore($"{directory} not Exist.");
            var names = TestEngine.GetTestFullNames(pathFile, directory);

            foreach (var name in names)
                Console.WriteLine($"{name}");

            Assert.AreEqual(4, names.Length);
            Assert.IsTrue(names.Any(e => e.Contains("(Application)")));
        }

        [TestCase(2021)]
        [TestCase(2022)]
        [TestCase(2023)]
        [TestCase(2024)]
        public void TestAssembly_Names_Resolver_DB(int version)
        {
            var pathFile = GetPathFile("Revit\\RevitTestDB.dll");
            var directory = GetResolverDirectory(version);
            if (!Directory.Exists(directory)) Assert.Ignore($"{directory} not Exist.");
            var names = TestEngine.GetTestFullNames(pathFile, directory);

            foreach (var name in names)
                Console.WriteLine($"{name}");

            Assert.AreEqual(2, names.Length);
            Assert.IsTrue(names.Any(e => e.Contains("(Application)")));
        }
    }
}
