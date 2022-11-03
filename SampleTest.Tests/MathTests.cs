using NUnit.Framework;
using System;

namespace SampleTest.Tests
{
    public class MathTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }

        [Test]
        public void TestSum()
        {
            var value = MathUtil.Sum(1, 2, 3);
            Assert.AreEqual(value, 6);
        }

        [Test]
        public void TestAverage()
        {
            var value = MathUtil.Average(1, 2, 3);
            Assert.AreEqual(value, 2);
        }

        [Test]
        public void TestInvert()
        {
            var value = MathUtil.Invert(2);
            Assert.AreEqual(value, 0.5);
        }

        [Test]
        public void TestInvertThrows()
        {
            Assert.Throws<DivideByZeroException>(() => { MathUtil.Invert(0); });
        }
    }
}