using NUnit.Framework;
using System;

namespace SampleTest.Tests
{
    [Explicit]
    public class TestsExplicit1
    {
        public TestsExplicit1()
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
    public class TestsExplicit2 : IDisposable
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
    public class TestsExplicit3
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
    public class TestsExplicit4
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
    public class TestsExplicit5
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
    public class TestsExplicit6
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
    public class TestsExplicit
    {
        [Test]
        public void TestParameterText(string text)
        {
            Console.WriteLine($"Value is {text}");
            Assert.IsNotNull(text);
            Assert.IsNotEmpty(text);
        }

        [Test]
        public void TestParameterInteger(int integer)
        {
            Console.WriteLine($"Value is {integer}");
            Assert.IsTrue(integer > 0);
        }
    }
}