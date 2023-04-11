using NUnit.Framework;
using System;

namespace SampleTest.Tests.TestAbstracts
{
    public class AbstractTest1 : TestsBase
    {

    }

    public class AbstractTest2 : TestsBase
    {

    }

    public class AbstractTest3 : TestsBase
    {

    }

    public abstract class TestsBase
    {
        [SetUp]
        public void SetUp()
        {
            Console.WriteLine("SetUp");
        }
        [Test]
        public void Tests_Abstract()
        {
            Console.WriteLine(this.GetType());
            Assert.Pass($"{this.GetType()} {this.GetType().Name}");
        }
        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("TearDown");
        }
    }
}