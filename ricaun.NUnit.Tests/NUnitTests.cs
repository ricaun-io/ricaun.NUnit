using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace ricaun.NUnit.Tests
{
    public class NUnitTests
    {
        private const string fileName = "ricaun.NUnit.1.0.6.dll";

        [Test]
        public void TestAssembly()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pathFile = Path.Combine(location, fileName);
            Console.WriteLine(fileName);
            //Console.WriteLine(pathFile);
            //Console.WriteLine(Assembly.GetExecutingAssembly().Location);
            var json = ricaun.NUnit.TestEngine.TestAssembly(pathFile);
            Console.WriteLine(json);
            Assert.IsTrue(json.Success);
        }
    }
}