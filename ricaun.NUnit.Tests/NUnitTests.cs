using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ricaun.NUnit.Tests
{
    public class NUnitTests
    {
        private const string fileName = "SampleTest.Tests.dll";
        private string pathFile;

        public NUnitTests()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            pathFile = Path.Combine(location, fileName);
        }

        [Test]
        public void TestAssembly_Initialize()
        {
            TestEngine.Initialize(out string message);
            Console.WriteLine(message);
            Assert.IsTrue(TestEngine.Initialize());
            Assert.IsNotNull(TestEngine.Version);
            Assert.IsNotNull(TestEngine.VersionNUnit);
        }

        [Test]
        public void TestAssembly_ContainNUnit()
        {
            Console.WriteLine(fileName);
            Assert.IsTrue(TestEngine.ContainNUnit(pathFile));
        }

        [Test]
        public void TestAssembly_Names()
        {
            Console.WriteLine(fileName);
            var names = TestEngine.GetTestFullNames(pathFile);
            foreach (var name in names)
            {
                Console.WriteLine($"{name}");
            }
        }

        [Test]
        public void TestAssembly_NamesAlias()
        {
            Console.WriteLine(fileName);
            var testModel = TestEngine.TestAssembly(pathFile);
            var names = TestEngine.GetTestFullNames(pathFile);
            var nameAlias = testModel.Tests.SelectMany(type => type.Tests.Select(test => TestEngine.GetTestFullName(type, test))).ToArray();

            foreach (var alias in nameAlias)
            {
                Console.WriteLine(alias);
                if (!names.Contains(alias))
                {
                    Assert.Fail($"{alias} not found.");
                }
            }

            Assert.AreEqual(names.Length, nameAlias.Length);
        }

        [Test]
        public void TestAssembly_NamesAlias_FullName()
        {
            Console.WriteLine(fileName);
            var testModel = TestEngine.TestAssembly(pathFile);
            var nameAlias = testModel.Tests.SelectMany(type => type.Tests.Select(test => TestEngine.GetTestFullName(type, test))).ToArray();
            var testFullNames = testModel.Tests.SelectMany(type => type.Tests.Select(test => test.FullName)).ToArray();
            foreach (var testFullName in testFullNames)
            {
                Console.WriteLine(testFullName);
            }
            Assert.IsTrue(nameAlias.SequenceEqual(testFullNames), "Sequence Alias and FullName equal");
        }

        [Test]
        public void TestAssembly_TestEngineResult()
        {
            var testCount = 0;
            var textOut = "";
            Console.WriteLine(fileName);
            TestEngineResult.Result = new TestModelResult((test) =>
            {
                testCount++;
                textOut += $"{testCount}\t{test.FullName}\n";
            });
            var testModel = TestEngine.TestAssembly(pathFile);
            //Console.WriteLine(testModel.AsString());
            Console.WriteLine(textOut);
            TestEngineResult.Result = null;

            Assert.AreEqual(testModel.TestCount, testCount);
        }

        [Test]
        public void TestAssembly_Service()
        {
            Console.WriteLine(fileName);
            var service = new Services.TestAssemblyService(pathFile);
            foreach (var method in service.GetTestTypeMethods())
            {
                var names = string.Join(" ", service.GetMethodTestNames(method));
                Console.WriteLine($"{service.GetMethodFullName(method)} \t{names}");
            }
        }

        [Test]
        public void TestAssembly()
        {
            Console.WriteLine(fileName);
            var testModel = TestEngine.TestAssembly(pathFile);
            var text = testModel.AsString();
            Console.WriteLine(text);
            Console.WriteLine(testModel.Message);
            Assert.IsTrue(testModel.TestCount > 0, $"{fileName} with no Tests.");
            Assert.IsTrue(testModel.Success, $"{fileName} Failed.");
        }

        [Test]
        public void TestAssembly_Explicit()
        {
            Console.WriteLine(fileName);
            TestEngineFilter.ExplicitEnabled = true;
            var testModel = TestEngine.TestAssembly(pathFile);
            var text = testModel.AsString();
            TestEngineFilter.Reset();
            Console.WriteLine(text);
            Console.WriteLine(testModel.Message);
            Assert.IsTrue(testModel.TestCount > 0, $"{fileName} with no Tests.");
        }

        [Test]
        public void TestAssembly_Parameters()
        {
            Console.WriteLine(fileName);
            TestEngineFilter.Add("*.TestParameter*");
            var testModel = TestEngine.TestAssembly(pathFile, "String Value", 10);
            var text = testModel.AsString();
            TestEngineFilter.Reset();
            Console.WriteLine(text);
            Console.WriteLine(testModel.Message);
            Assert.IsTrue(testModel.TestCount > 0, $"{fileName} with no Tests.");
        }

        [TestCase("*.TestPass", 1)]
        [TestCase("*.TestReturn", 1)]
        [TestCase("*.TestExplicit", 1)]
        [TestCase("*.TestName?", 2)]
        [TestCase("*.TestCases(?)", 2)]
        [TestCase("*.TestSame?", 2)]
        [TestCase("*", 32)]
        public void TestAssembly_Filter(string testName, int numberOfTests)
        {
            TestEngineFilter.Add(testName);
            var testModel = TestEngine.TestAssembly(pathFile);
            TestEngineFilter.Reset();

            var text = testModel.AsString();
            Console.WriteLine(text);
            Assert.AreEqual(numberOfTests, testModel.TestCount, $"{fileName} with no Tests.");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        public void TestAssembly_FilterName(int numberOfTests)
        {
            Console.WriteLine(fileName);
            var names = TestEngine.GetTestFullNames(pathFile);
            for (int i = 0; i < numberOfTests; i++)
            {
                TestEngineFilter.Add(names[i]);
            }
            var testModel = TestEngine.TestAssembly(pathFile);
            TestEngineFilter.Reset();

            var text = testModel.AsString();
            Console.WriteLine(text);
            Assert.AreEqual(numberOfTests, testModel.TestCount, $"{fileName} with no Tests.");
        }
    }
}