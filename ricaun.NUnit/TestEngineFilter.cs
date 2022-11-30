using System.Collections.Generic;

namespace ricaun.NUnit
{
    /// <summary>
    /// TestEngineFilter
    /// </summary>
    public static class TestEngineFilter
    {
        /// <summary>
        /// TestNames
        /// </summary>
        public static List<string> TestNames { get; private set; } = new List<string>();

        /// <summary>
        /// ExplicitEnabled (default: false)
        /// </summary>
        public static bool ExplicitEnabled { get; set; } = false;

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="name"></param>
        public static void Add(string name)
        {
            TestNames.Add(name);
        }
        /// <summary>
        /// Clear
        /// </summary>
        public static void Clear()
        {
            TestNames.Clear();
        }
        /// <summary>
        /// Reset
        /// </summary>
        public static void Reset()
        {
            Clear();
            ExplicitEnabled = false;
        }
        /// <summary>
        /// HasName
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HasName(string name)
        {
            if (TestNames.Count == 0)
                return true;

            return TestNames.Contains(name);
        }
    }
}
