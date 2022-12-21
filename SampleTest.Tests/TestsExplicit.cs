using NUnit.Framework;
using System;

namespace SampleTest.Tests
{
    [Explicit]
    public class TestsExplicit1_ShouldFail
    {
        public TestsExplicit1_ShouldFail()
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
    public class TestsExplicit2_ShouldFail : IDisposable
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
    public class TestsExplicit3_ShouldFail
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
    public class TestsExplicit4_ShouldFail
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
    public class TestsExplicit5_ShouldFail
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
    public class TestsExplicit6_ShouldFail
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
    public class TestsExplicit_ShouldFail
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