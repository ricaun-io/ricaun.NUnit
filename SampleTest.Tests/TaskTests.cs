using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace SampleTest.Tests
{
    public class TaskTests
    {
        [Test]
        public async Task TestTask()
        {
            Console.WriteLine("TestTask");
            await Task.Delay(10);
            Assert.Pass("TestTask");
        }

        [Test(ExpectedResult = 1)]
        public async Task<int> TestTaskInteger()
        {
            Console.WriteLine("TestTaskInteger");
            await Task.Delay(10);
            return 1;
        }

        [Test(ExpectedResult = 1)]
        public async Task<object> TestTaskObject()
        {
            Console.WriteLine("TestTaskObject");
            await Task.Delay(10);
            return 1;
        }

        [TestCase(1, ExpectedResult = 1)]
        public async Task<int> TestTaskIntegerValue(int value)
        {
            Console.WriteLine("TestTaskIntegerValue");
            await Task.Delay(10);
            return value;
        }

        [TestCase(1, ExpectedResult = 1)]
        public async Task<object> TestTaskObjectValue(object value)
        {
            Console.WriteLine("TestTaskObjectValue");
            await Task.Delay(10);
            return value;
        }

        [TestCase(100)]
        [TestCase(500)]
        [TestCase(900)]
        public async Task TestTaskTimeOut(int delay)
        {
            Console.WriteLine("TestTaskTimeOut");
            await Task.Delay(delay);
            Assert.Pass("TestTaskTimeOut");
        }
    }
}
