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
        [SetUp]
        public void SetUp2()
        {
            Console.WriteLine("SetUp2");
            Name += "b";
        }
        [TearDown]
        public void TearDown2()
        {
            Console.WriteLine("TearDown2");
        }
    }

    public class AbstractTest4 : TestsBase
    {
        [SetUp]
        public void SetUp2()
        {
            Console.WriteLine("SetUp2");
            Name += "b";
        }
        [SetUp]
        public void SetUp3()
        {
            Console.WriteLine("SetUp3");
            Name += "c";
        }
        [TearDown]
        public void TearDown2()
        {
            Console.WriteLine("TearDown2");
        }
        [TearDown]
        public void TearDown3()
        {
            Console.WriteLine("TearDown3");
        }


        [OneTimeSetUp]
        public void OneTimeSetUp2()
        {
            Console.WriteLine("OneTimeSetUp2");
            Name += "2";
        }
        [OneTimeTearDown]
        public void OneTimeTearDown2()
        {
            Console.WriteLine("OneTimeTearDown2");
        }
        [OneTimeSetUp]
        public void OneTimeSetUp3()
        {
            Console.WriteLine("OneTimeSetUp3");
            Name += "3";
        }
        [OneTimeTearDown]
        public void OneTimeTearDown3()
        {
            Console.WriteLine("OneTimeTearDown3");
        }
    }

    public abstract class TestsBase
    {
        protected string Name = "";
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Console.WriteLine("OneTimeSetUp");
            Name += "1";
        }
        [SetUp]
        public void SetUp()
        {
            Console.WriteLine("SetUp");
            Name += "a";
        }
        [Test]
        public void Tests_Abstract()
        {
            Console.WriteLine($"{this.GetType()} - {Name}");
            Assert.Pass($"{this.GetType()} {this.GetType().Name}");
        }
        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("TearDown");
        }
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Console.WriteLine("OneTimeTearDown");
        }
    }
}