using NUnit.Framework;
using System;

namespace SampleTest.Tests
{
    [TestFixture(1)]
    [TestFixture(2)]
    [TestFixture(3)]
    public class TestsFixture
    {
        private int value;

        public TestsFixture(int value)
        {
            this.value = value;
        }
        [Test]
        public void TestValue()
        {
            Console.WriteLine(value);
            Assert.NotZero(value);
        }
    }

    [TestFixtureSource(nameof(FixtureArgs))]
    public class TestsFixtureSource
    {
        public static object[] FixtureArgs = { 1, 2, 3 };

        private int value;

        public TestsFixtureSource(int value)
        {
            this.value = value;
        }
        [Test]
        public void TestValue()
        {
            Console.WriteLine(value);
            Assert.NotZero(value);
        }
    }
}