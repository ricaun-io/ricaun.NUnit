using NUnit.Framework;
using System.Collections.Generic;

namespace SampleTest.Tests
{
    public class TestsCaseSource
    {

        public static int[] CasesSource = new[] { 1, 2, 3 };
        [TestCaseSource(nameof(CasesSource))]
        public void CasesSourceTest(int i)
        {
            Assert.True(i > 0);
        }


        static IEnumerable<int> CasesSourceMethod()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }
        [TestCaseSource(nameof(CasesSourceMethod))]
        public void CasesSourceMethodTest(int i)
        {
            Assert.True(i > 0);
        }

        static IEnumerable<int> CasesSourceMethodWithParameters(int start, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return start + i;
            }
        }

        [TestCaseSource(nameof(CasesSourceMethodWithParameters), new object[] { 1, 4 })]
        public void CasesSourceMethodWithParametersTest(int i)
        {
            Assert.True(i > 0);
        }
    }
}