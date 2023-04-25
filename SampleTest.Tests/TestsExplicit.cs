using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleTest.Tests.Explicits
{
    [Explicit]
    public class TestsExplicit1_ShouldFail_Constructor
    {
        public TestsExplicit1_ShouldFail_Constructor()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void TestConstructor()
        {
            Console.WriteLine("Failed");
        }
    }

    [Explicit]
    public class TestsExplicit2_ShouldFail_Dispose : IDisposable
    {
        [Test]
        public void TestDispose()
        {
            Console.WriteLine("Passed");
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    [Explicit]
    public class TestsExplicit3_ShouldFail_OneTimeSetUp
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void TestSetUp()
        {
            Console.WriteLine("Failed");
        }
    }

    [Explicit]
    public class TestsExplicit4_ShouldFail_OneTimeTearDown
    {
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void TestTearDown()
        {
            Console.WriteLine("Passed");
        }
    }

    [Explicit]
    public class TestsExplicit5_ShouldFail_SetUp
    {
        [SetUp]
        public void SetUp()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void TestSetUp()
        {
            Console.WriteLine("Failed");
        }
    }

    [Explicit]
    public class TestsExplicit6_ShouldFail_TearDown
    {
        [TearDown]
        public void TearDown()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void TestTearDown()
        {
            Console.WriteLine("Failed");
        }
    }

    [Explicit]
    public class TestsExplicit7_ShouldFail_Parameter
    {
        [Test]
        public void TestParameterText(string value)
        {
            Console.WriteLine($"Value is {value}");
            Assert.IsNotNull(value);
            Assert.IsNotEmpty(value);
            Assert.Pass($"{value} {value.GetType()}");
        }

        [Test]
        public void TestParameterInteger(int value)
        {
            Console.WriteLine($"Value is {value}");
            Assert.IsTrue(value > 0);
            Assert.Pass($"{value} {value.GetType()}");
        }

        [Test]
        public void TestParameterLong(long value)
        {
            Console.WriteLine($"Value is {value}");
            Assert.IsTrue(value > 0);
            Assert.Pass($"{value} {value.GetType()}");
        }

        [Test]
        public void TestParameterMultiple(int value, long value2)
        {
            Console.WriteLine($"Value is {value}");
            Console.WriteLine($"Value2 is {value2}");
            Assert.IsTrue(value > 0);
            Assert.IsTrue(value2 > 0);
            Assert.Pass($"{value} {value2}");
        }

        [Test]
        public void TestParameterArray(string[] value)
        {
            Console.WriteLine($"Value is {value}");
            Assert.IsNotNull(value);
            Assert.IsNotEmpty(value);
            Assert.Pass($"{value} {value.GetType()}");
        }

        [Test]
        public void TestParameterIEnumerable(IEnumerable<string> value)
        {
            Console.WriteLine($"Value is {value}");
            Assert.IsNotNull(value);
            Assert.IsNotEmpty(value);
            Assert.Pass($"{value} {value.GetType()}");
        }

        [Test]
        public void TestParameterInterface(ICloneable value)
        {
            Console.WriteLine($"Value is {value}");
            Assert.IsNotNull(value);
            Assert.Pass($"{value} {value.GetType()}");
        }
    }

    [Explicit]
    public class TestsExplicit8_ShouldPass_Case
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void TestParameterCaseText(string value)
        {
            Console.WriteLine($"Value is {value}");
            Assert.IsTrue(string.IsNullOrWhiteSpace(value));
            Assert.Pass($"{value} {value?.GetType()}");
        }

        [TestCase("*")]
        [TestCase(".")]
        [TestCase("°C")]
        public void TestParameterCaseText_IsNot(string value)
        {
            Console.WriteLine($"Value is {value}");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(value));
            Assert.Pass($"{value} {value?.GetType()}");
        }

        [TestCase(1)]
        public void TestParameterCaseInteger(int value)
        {
            Console.WriteLine($"Value is {value}");
            Assert.Pass($"{value} {value.GetType()}");
        }
    }
}