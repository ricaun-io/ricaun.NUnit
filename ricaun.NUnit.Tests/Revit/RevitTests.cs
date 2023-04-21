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
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(location, $"Revit\\{version}\\RevitTest1.dll");
        }
        #endregion

        [TestCase(2021)]
        [TestCase(2022)]
        [TestCase(2023)]
        [TestCase(2024)]
        public void TestAssembly_ContainNUnit(int version)
        {
            var pathFile = GetPathFile(version);
            Assert.IsTrue(TestEngine.ContainNUnit(pathFile));
        }

        [TestCase(2021)]
        [TestCase(2022)]
        [TestCase(2023)]
        [TestCase(2024)]
        public void TestAssembly_Names(int version)
        {
            var pathFile = GetPathFile(version);
            var names = TestEngine.GetTestFullNames(pathFile);

            foreach (var name in names)
                Console.WriteLine($"{name}");

            Assert.AreEqual(4, names.Length);
        }

        [TestCase(2021)]
        [TestCase(2022)]
        [TestCase(2023)]
        [TestCase(2024)]
        public void TestAssembly_Names_Resolver(int version)
        {
            var pathFile = GetPathFile(version);
            var directory = GetResolverDirectory(version);
            if (!Directory.Exists(directory)) Assert.Ignore($"{directory} not Exist.");
            var names = TestEngine.GetTestFullNames(pathFile, directory);

            foreach (var name in names)
                Console.WriteLine($"{name}");

            Assert.AreEqual(4, names.Length);
        }
    }
}
