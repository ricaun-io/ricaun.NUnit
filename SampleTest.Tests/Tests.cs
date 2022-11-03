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
    }
}