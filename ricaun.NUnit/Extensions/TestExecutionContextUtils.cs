using NUnit.Framework.Internal;
using System;

namespace ricaun.NUnit.Extensions
{
    /// <summary>
    /// TestExecutionContextUtils
    /// </summary>
    internal static class TestExecutionContextUtils
    {
        /// <summary>
        /// Clear <see cref="TestExecutionContext"/>
        /// </summary>
        public static void Clear()
        {
            TestExecutionContext.CurrentContext.CurrentResult.AssertionResults.Clear();
        }
    }
}
