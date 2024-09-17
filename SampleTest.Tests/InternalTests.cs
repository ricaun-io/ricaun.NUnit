using NUnit.Framework;

namespace SampleTest.Tests
{
    internal class InternalTests
    {
        [Test]
        public void PublicTest() { }

        [Test]
        internal void InternalTest() { }

        [Test]
        private void PrivateTest() { }
    }
}
