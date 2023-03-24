using NUnit.Framework;
using System;

namespace SampleTest.Tests
{
    public class Tests
    {
        [Test]
        public void TestPass()
        {
            Console.WriteLine("Pass Message");
            Assert.Pass("Pass Message");
        }

        [Test]
        public void TestFail()
        {
#if FAIL
            Assert.Fail("Fail Message");
#endif
        }

        [Test]
        public void TestIgnore()
        {
            Assert.Ignore("Ignore Message");
        }

        [Ignore("Ignore")]
        [Test]
        public void TestIgnore2()
        {

        }

        [Test]
        [Explicit("This is Explicit")]
        public void TestExplicit()
        {

        }

        [Test(ExpectedResult = 1)]
        public int TestReturn()
        {
            return 1;
        }

        [Test]
        public void TestConsole()
        {
            Console.WriteLine("Console");
        }

        [Test]
        public void TestNormal()
        {
            var a = 0;
            for (int i = 0; i < 10000000; i++)
            {
                a += i;
            }
            Assert.True(true);
        }

        [TestCase(1, TestName = "TestName1")]
        [TestCase(2, TestName = "TestName2")]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5, TestName = "TestSame2")]
        public void TestCases(int i)
        {
            Assert.True(i > 0);
        }

        [Test]
        public void TestSame1()
        {

        }
    }
}