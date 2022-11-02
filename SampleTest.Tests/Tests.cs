using NUnit.Framework;
using System;

namespace SampleTest.Tests
{
    public class TestConstructor : IDisposable
    {
        public TestConstructor()
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [Test]
        public void Test()
        {
        }
    }

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
        public void TestFail2()
        {
#if FAIL
            Assert.Fail("Fail2 Message");
#endif
        }

        [Test]
        public void TestIgnore()
        {
            Assert.Ignore();
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