using NUnit.Framework;

namespace SampleTest.Tests
{
    internal class InternalTests
    {
        [Test]
        public void PublicTest() { }

#if DEBUG
        // NUnit throw 'Method is not public'
        [Test]
        internal void InternalTest() { }

        [Test]
        private void PrivateTest() { }
#endif
    }
}
