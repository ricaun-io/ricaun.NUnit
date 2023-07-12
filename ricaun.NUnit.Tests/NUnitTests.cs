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
        public void TestAssembly_ContainNUnit_Version()
        {
            Console.WriteLine(fileName);
            Assert.IsTrue(TestEngine.ContainNUnit(pathFile, out AssemblyName assemblyName));
            Console.WriteLine(assemblyName);
            Assert.AreEqual(TestEngine.VersionNUnit, assemblyName.Version);
        }

        [Test]
        public void TestAssembly_Names()
        {
            var names = TestEngine.GetTestFullNames(pathFile);
            Console.WriteLine($"{fileName} {names.Length}");
            foreach (var name in names)
            {
                Console.WriteLine($"{name}");
            }
            Assert.IsNotEmpty(names);
        }

        [Test]
        public void TestAssembly_Fail()
        {
            var names = TestEngine.GetTestFullNames(pathFile);
            var model = TestEngine.Fail(pathFile, new Exception());
            var tests = model.Tests.SelectMany(type => type.Tests).ToArray();
            foreach (var test in tests)
            {
                Console.WriteLine(test);
            }
            Assert.AreEqual(names.Length, tests.Length);
        }

        [Test]
        public void TestAssembly_Fail_Names()
        {
            var names = new[] { "Test", "Test2" };
            var model = TestEngine.Fail(pathFile, new Exception(), names);
            var tests = model.Tests.SelectMany(type => type.Tests).ToArray();
            foreach (var test in tests)
            {
                Console.WriteLine(test);
            }
            Assert.AreEqual(names.Length, tests.Length);
        }

        [Test]
        public void TestAssembly_Names_Resolver()
        {
            Console.WriteLine(fileName);
            var names = TestEngine.GetTestFullNames(pathFile, "");
            foreach (var name in names)
            {
                Console.WriteLine($"{name}");
            }
            Assert.IsNotEmpty(names);
        }

        [Test]
        public void TestAssembly_NamesAlias_FullName_EqualsType()
        {
            Console.WriteLine(fileName);
            var testModel = TestEngine.TestAssembly(pathFile);

            foreach (var testType in testModel.Tests)
            {
                foreach (var test in testType.Tests)
                {
                    var startWithType = test.FullName.StartsWith(testType.FullName);
                    Assert.IsTrue(startWithType, $"{test.FullName} not start with {testType.FullName}");
                }
            }
        }

        [Test]
        public void TestAssembly_NamesAlias()
        {
            var testFullNames = TestEngine.GetTestFullNames(pathFile);
            var testModel = TestEngine.TestAssembly(pathFile);
            var testFullNamesTwo = testModel.Tests.SelectMany(type => type.Tests.Select(test => test.FullName)).ToArray();
            Console.WriteLine($"{fileName} {testFullNamesTwo.Length}");

            Assert.AreEqual(testFullNamesTwo.Length, testFullNames.Length);

            for (int i = 0; i < testFullNamesTwo.Length; i++)
            {
                Console.WriteLine($"{testFullNamesTwo[i]} \t {testFullNames[i]}");
            }

            Assert.IsTrue(testFullNamesTwo.SequenceEqual(testFullNames), "Sequence Alias and FullName equal");
        }

        [Test]
        public void TestAssembly_NamesAlias_FullName()
        {
            Console.WriteLine(fileName);
            var testModel = TestEngine.TestAssembly(pathFile);
#pragma warning disable CS0618 // Type or member is obsolete
            var nameAlias = testModel.Tests.SelectMany(type => type.Tests.Select(test => TestEngine.GetTestFullName(type, test))).ToArray();
#pragma warning restore CS0618 // Type or member is obsolete
            var testFullNames = testModel.Tests.SelectMany(type => type.Tests.Select(test => test.FullName)).ToArray();

            Assert.AreEqual(nameAlias.Length, testFullNames.Length);

            for (int i = 0; i < nameAlias.Length; i++)
            {
                Console.WriteLine($"{nameAlias[i]} \t {testFullNames[i]}");
            }

            Assert.IsTrue(nameAlias.SequenceEqual(testFullNames), "Sequence Alias and FullName equal");
        }

        [Test]
        public void TestAssembly_TestEngineResult()
        {
            var testCount = 0;
            var textOut = "";
            var tick = DateTime.Now.Ticks;

            TestEngine.Result = new TestModelResult((test) =>
            {
                testCount++;
                textOut += $"{testCount}\t{DateTime.Now.Ticks - tick}\t{test.FullName}\n";
            });
            var testModel = TestEngine.TestAssembly(pathFile);
            Console.WriteLine(textOut);
            TestEngine.Result = null;

            Assert.AreEqual(testModel.TestCount, testCount);
        }

        [Test]
        public void TestAssembly_Service()
        {
            Console.WriteLine(fileName);
            var service = new Services.TestAssemblyService(pathFile);
            var typeMethods = service.GetTestDictionaryTypeMethods();
            foreach (var typeMethod in typeMethods)
            {
                var type = typeMethod.Key;
                var methods = typeMethod.Value;
                foreach (var method in methods)
                {
                    var names = string.Join(" ", service.GetMethodTestNames(method));
                    Console.WriteLine($"{service.GetMethodFullName(type, method)} \t{names}");
                }
            }
            Assert.IsNotEmpty(typeMethods);
        }

        [Test]
        public void TestAssembly_AbstractTest()
        {
            Console.WriteLine(fileName);
            TestEngineFilter.Add("*Abstract*");
            var testModel = TestEngine.TestAssembly(pathFile);
            TestEngineFilter.Reset();
            var text = testModel.AsString();
            Console.WriteLine(text);
            Console.WriteLine(testModel.Message);
            Console.WriteLine();
            foreach (var test in testModel.Tests.SelectMany(e => e.Tests))
            {
                Console.WriteLine($"{test.Name}");
                Console.WriteLine($"{test.Console}");
            }
            Assert.IsTrue(testModel.TestCount > 0, $"{fileName} with no Tests.");
            Assert.IsTrue(testModel.Success, $"{fileName} Failed.");
        }

        [Test]
        public void TestAssembly_MultipleTest()
        {
            Console.WriteLine(fileName);
            TestEngineFilter.Add("*.MultipleTest.*");
            var testModel = TestEngine.TestAssembly(pathFile);
            TestEngineFilter.Reset();
            var text = testModel.AsString();
            Console.WriteLine(text);
            Console.WriteLine(testModel.Message);
            Assert.IsTrue(testModel.TestCount > 0, $"{fileName} with no Tests.");
            Assert.IsTrue(testModel.Success, $"{fileName} Failed.");
        }

        [Test]
        public void TestAssembly_OrderTest()
        {
            Console.WriteLine(fileName);
            TestEngineFilter.Add("*.OrderTest.*");
            var testModel = TestEngine.TestAssembly(pathFile);
            TestEngineFilter.Reset();
            var text = testModel.AsString();
            Console.WriteLine(text);
            Console.WriteLine(testModel.Message);
            Assert.IsTrue(testModel.TestCount > 0, $"{fileName} with no Tests.");
            Assert.IsTrue(testModel.Success, $"{fileName} Failed.");
        }

        [Test(ExpectedResult = 13)]
        public int TestAssembly_Explicit_ShouldFail()
        {
            Console.WriteLine(fileName);
            TestEngineFilter.ExplicitEnabled = true;
            var testModel = TestEngine.TestAssembly(pathFile);
            var text = testModel.AsString();
            TestEngineFilter.Reset();
            Console.WriteLine(text);
            Console.WriteLine(testModel.Message);
            Assert.IsTrue(testModel.TestCount > 0, $"{fileName} with no Tests.");

            var failExplictTests = testModel.Tests.SelectMany(e => e.Tests).Count(e => e.Success == false);

            //var failExplictTests = testModel.TestCount - (int)(testModel.TestCount * testModel.SuccessHate);
            return failExplictTests;
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

        class BaseCloneable : ICloneable
        {
            public object Clone()
            {
                return this;
            }
            public override string ToString()
            {
                return nameof(BaseCloneable);
            }
        }

        [Test]
        public void TestAssembly_Parameters()
        {
            Console.WriteLine(fileName);
            TestEngineFilter.Add("*.TestParameter*");

            var array = new string[] { "Array1" };
            var testModel = TestEngine.TestAssembly(pathFile, new BaseCloneable(), "Text", 123, (long)456, array);
            var text = testModel.AsString();
            TestEngineFilter.Reset();
            Console.WriteLine(text);
            Console.WriteLine(testModel.Message);
            Assert.IsTrue(testModel.TestCount > 0, $"{fileName} with no Tests.");
            Assert.IsTrue(testModel.Success, $"{fileName} Failed.");
        }

        [TestCase("*.TestPass", 1)]
        [TestCase("*.TestReturn", 1)]
        [TestCase("*.TestExplicit", 1)]
        [TestCase("*.TestName?", 2)]
        [TestCase("*.TestCases(?)", 2)]
        [TestCase("*.TestSame?", 2)]
        [TestCase("*.TestTask*", 8)]
        [TestCase("*.OrderTest.*", 2)]
        [TestCase("*.MultipleTest.*", 2)]
        [TestCase("*AbstractTest?*", 4)]
        [TestCase("*Abstract*", 4)]
        [TestCase("*", 60)]
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