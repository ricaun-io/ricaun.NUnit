using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ricaun.NUnit.Revit
{
    public class TestSampleClass
    {
        [OneTimeSetUp]
        public void OneBeforeTest()
        {
            Console.WriteLine("Execute OneBeforeTest");
        }

        [OneTimeTearDown]
        public void OneAfterTest()
        {
            Console.WriteLine("Execute OneAfterTest");
        }

        [SetUp]
        public void BeforeTest()
        {
            Console.WriteLine("Execute BeforeTest");
        }

        [TearDown]
        public void AfterTest()
        {
            Console.WriteLine("Execute AfterTest");
        }

        [Test]
        public void NormalTest()
        {
            Console.WriteLine("Execute NormalTest");
            Assert.True(true);
        }

        [Test]
        [Explicit]
        public void FailTest()
        {
            Console.WriteLine("Execute FailTest");
            Assert.True(false, "This is a custom fail message.");
        }

        [Test]
        public void PassTest()
        {
            Console.WriteLine("Execute PassTest");
            Assert.Pass("This is a custom pass message.");
        }

        [Test]
        public async Task TaskTest()
        {
            await Task.Delay(500);
            Console.WriteLine("Execute TaskTest");
        }

        [TestCase(3000, ExpectedResult = 3000)]
        [TestCase(200, ExpectedResult = 200)]
        [TestCase(100, ExpectedResult = 100)]
        public async Task<object> TaskTestObject(int value)
        {
            await Task.Delay(value);
            return value;
        }
    }
}
