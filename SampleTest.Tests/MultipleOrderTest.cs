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
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Name += "c";
        }
        [OneTimeTearDown]
        public void OneTimeTearDownTwo()
        {
            Name += "d";
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
        [TearDown]
        public void TearDownTwo()
        {
            Name += "D";
        }
        [TearDown]
        public void TearDown()
        {
            Name += "C";
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
            Assert.AreEqual("abABCDAB", Name);
        }
    }
}