using NUnit.Framework;
using System;

namespace SampleTest.Tests
{
    public class MultipleOrderTest
    {
        string Name = "";
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Name += "a";
        }
        [OneTimeSetUp]
        public void OneTimeSetUpTwo()
        {
            Name += "b";
        }
        [SetUp]
        public void SetUp()
        {
            Name += "A";
        }
        [SetUp]
        public void SetUpTwo()
        {
            Name += "B";
        }
        [Test]
        [Order(1)]
        public void Test1()
        {
            Console.WriteLine(Name);
            Assert.AreEqual("abAB", Name);
        }
        [Test]
        [Order(2)]
        public void Test0()
        {
            Console.WriteLine(Name);
            Assert.AreEqual("abABAB", Name);
        }
    }
}