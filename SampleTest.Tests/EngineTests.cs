using NUnit.Framework;
using System;

namespace SampleTest.Tests
{
    public class EngineTests
    {

        public static int MyProperty { get; set; }

        [Test]
        public void Test1()
        {
            Console.WriteLine($"Test1: {MyProperty++}");
        }

        [Test]
        public void Test3()
        {
            Console.WriteLine($"Test3: {MyProperty++}");
        }

        [Test]
        public void Test2()
        {
            Console.WriteLine($"Test2: {MyProperty++}");
        }
    }
}