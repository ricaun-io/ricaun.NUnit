using NUnit.Framework;
using System;

namespace SampleTest.Tests.TestAbstracts
{
    public class Abstract1Tests : TestsBase
    {

    }

    public class Abstract2Tests : TestsBase
    {

    }

    public class Abstract3Tests : TestsBase
    {

    }

    public abstract class TestsBase
    {
        [SetUp]
        public void SetUp()
        {

        }
        [Test]
        public void Tests()
        {
            Console.WriteLine(this.GetType());
            Assert.Pass($"{this.GetType()} {this.GetType().Name}");
        }
        [TearDown]
        public void TearDown()
        {

        }
    }
}