using NUnit.Framework;

namespace ricaun.NUnit.Tests
{
    public class Tests
    {
        [Test]
        public void NormalTest()
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