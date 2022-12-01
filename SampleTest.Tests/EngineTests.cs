using NUnit.Framework;
using System;

namespace SampleTest.Tests
{
    public class EngineTests
    {
        public int Property { get; set; }

        [Test]
        public void Test1()
        {
            Console.WriteLine($"Test1: {++Property}");
            Assert.AreEqual(1, Property);
        }

        [Test]
        public void Test3()
        {
            Console.WriteLine($"Test3: {++Property}");
            Assert.AreEqual(3, Property);
        }

        [Test]
        public void Test2()
        {
            Console.WriteLine($"Test2: {++Property}");
            Assert.AreEqual(2, Property);
        }
    }
}