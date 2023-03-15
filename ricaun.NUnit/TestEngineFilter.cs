using ricaun.NUnit.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ricaun.NUnit
{
    /// <summary>
    /// TestEngineFilter
    /// </summary>
    public static class TestEngineFilter
    {
        /// <summary>
        /// CancellationToken TimeOut for Task Tests (default: 1 Minute)
        /// </summary>
        public static TimeSpan CancellationTokenTimeOut { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// TestNames
        /// </summary>
        public static List<string> TestNames { get; private set; } = new List<string>();

        /// <summary>
        /// ExplicitEnabled (default: false)
        /// </summary>
        public static bool ExplicitEnabled { get; set; } = false;

        /// <summary>
        /// Add Test <paramref name="wildcard"/> and enable <see cref="ExplicitEnabled"/>
        /// </summary>
        /// <param name="wildcard"></param>
        public static void Add(string wildcard)
        {
            if (string.IsNullOrEmpty(wildcard)) return;
            ExplicitEnabled = true;
            TestNames.Add(wildcard);
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

            return TestNames.Any(e => WildcardPattern.Match(e, name));
        }
    }
}
