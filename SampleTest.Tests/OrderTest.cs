using NUnit.Framework;
using System;

namespace SampleTest.Tests
{
    public class OrderTest
    {
        string Name = "1";
        [Test]
        [Order(1)]
        public void Test1()
        {
            Console.WriteLine(Name);
            Assert.AreEqual("1", Name);
            Name += "2";
        }
        [Test]
        [Order(2)]
        public void Test0()
        {
            Console.WriteLine(Name);
            Assert.AreEqual("12", Name);
        }
    }
}