using ricaun.NUnit.Models;
using System;

namespace ricaun.NUnit
{
    /// <summary>
    /// TestEngineResult
    /// </summary>
    public class TestEngineResult
    {
        /// <summary>
        /// InvokeResult
        /// </summary>
        /// <param name="testModel"></param>
        internal static void InvokeResult(TestModel testModel)
        {
            Result?.Result(testModel);
        }

        /// <summary>
        /// ITestModelResult
        /// </summary>
        public static ITestModelResult Result { get; set; }
    }

    /// <summary>
    /// TestModelResult
    /// </summary>
    public class TestModelResult : ITestModelResult
    {
        private readonly Action<TestModel> Action;
        /// <summary>
        /// TestModelResult
        /// </summary>
        /// <param name="action"></param>
        public TestModelResult(Action<TestModel> action)
        {
            Action = action;
        }

        /// <summary>
        /// Result
        /// </summary>
        /// <param name="testModel"></param>
        public void Result(TestModel testModel)
        {
            Action?.Invoke(testModel);
        }
    }

    /// <summary>
    /// ITestModelResult
    /// </summary>
    public interface ITestModelResult
    {
        /// <summary>
        /// Result
        /// </summary>
        /// <param name="testModel"></param>
        public void Result(TestModel testModel);
    }
}
