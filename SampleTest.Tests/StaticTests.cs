using NUnit.Framework;

namespace SampleTest.Tests
{
    public class StaticTests
    {
        public static int PropertyOneTime { get; set; }
        public static int PropertyNormal { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            PropertyOneTime = 0;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Assert.AreEqual(3, PropertyOneTime);
        }

        [SetUp]
        public void SetUp()
        {
            PropertyNormal = 0;
        }

        [TearDown]
        public void TearDown()
        {
            Assert.AreEqual(1, PropertyNormal);
        }

        [Test]
        public void TestStatic1()
        {
            Add();
            Assert.AreEqual(1, PropertyOneTime);
        }

        [Test]
        public void TestStatic2()
        {
            Add();
            Assert.AreEqual(2, PropertyOneTime);
        }
        [Test]
        public void TestStatic3()
        {
            Add();
            Assert.AreEqual(3, PropertyOneTime);
        }

        public void Add()
        {
            PropertyOneTime++;
            PropertyNormal++;
        }
    }
}