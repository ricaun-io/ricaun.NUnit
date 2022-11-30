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
        /// Add Test <paramref name="name"/> and enable <see cref="ExplicitEnabled"/>
        /// </summary>
        /// <param name="name"></param>
        public static void Add(string name)
        {
            ExplicitEnabled = true;
            TestNames.Add(name);
        }
        /// <summary>
        /// Reset and disable <see cref="ExplicitEnabled"/>
        /// </summary>
        public static void Reset()
        {
            TestNames.Clear();
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
