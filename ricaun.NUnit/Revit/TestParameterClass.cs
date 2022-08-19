using NUnit.Framework;
using System;

namespace ricaun.NUnit.Revit
{
    public class TestParameterClass : IDisposable
    {
        public TestParameterClass(string parameter)
        {
            Console.WriteLine($"Constructor TestParameterClass: {parameter}");
        }

        [Test]
        public void ParameterTest(string parameter)
        {
            Console.WriteLine($"ParameterTest: {parameter}");
            Assert.IsNotNull(parameter);
        }

        public void Dispose()
        {
            Console.WriteLine($"Dispose TestParameterClass");
        }
    }
}
