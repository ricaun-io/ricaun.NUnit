﻿using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
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
        public void TestAssemblyNames()
        {
            Console.WriteLine(fileName);
            var service = new Services.TestAssemblyService(pathFile);
            foreach (var names in service.GetTestFullNames())
            {
                Console.WriteLine($"{names}");
            }
        }

        [Test]
        public void TestAssemblyService()
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
            var json = TestEngine.TestAssembly(pathFile);
            var text = json.AsString();
            Console.WriteLine(text);

            Assert.IsTrue(json.TestCount > 0, $"{fileName} with no Tests.");
        }

        [Test]
        public void TestAssemblyExplicit()
        {
            Console.WriteLine(fileName);
            TestEngineFilter.ExplicitEnabled = true;
            var json = TestEngine.TestAssembly(pathFile);
            var text = json.AsString();
            TestEngineFilter.Reset();
            Console.WriteLine(text);

            Assert.IsTrue(json.TestCount > 0, $"{fileName} with no Tests.");
        }

        [TestCase("*.Test1", 1)]
        [TestCase("*.Test2", 1)]
        [TestCase("*.Test3", 1)]
        [TestCase("*.TestName?", 2)]
        [TestCase("*.TestCases(?)", 2)]
        [TestCase("*.TestSame", 2)]
        [TestCase("*", 20)]
        public void TestAssemblyFilter(string testName, int numberOfTests)
        {
            TestEngineFilter.Add(testName);
            var json = TestEngine.TestAssembly(pathFile);
            TestEngineFilter.Reset();

            var text = json.AsString();
            Console.WriteLine(text);
            Assert.AreEqual(numberOfTests, json.TestCount, $"{fileName} with no Tests.");
        }

    }
}