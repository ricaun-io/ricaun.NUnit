using System;
using System.Text.RegularExpressions;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// WildcardPattern
    /// <code>https://stackoverflow.com/questions/30299671/matching-strings-with-wildcard</code>
    /// </summary>
    internal class WildcardPattern
    {
        private readonly string _expression;
        private readonly Regex _regex;

        /// <summary>
        /// WildcardPattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public WildcardPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) throw new ArgumentNullException(nameof(pattern));

            _expression = "^" + Regex.Escape(pattern)
                .Replace("\\\\\\?", "??").Replace("\\?", ".").Replace("??", "\\?")
                .Replace("\\\\\\*", "**").Replace("\\*", ".*").Replace("**", "\\*") + "$";
            _regex = new Regex(_expression, RegexOptions.Compiled);
        }

        /// <summary>
        /// IsMatch
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsMatch(string value)
        {
            return _regex.IsMatch(value);
        }

        /// <summary>
        /// Match
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Match(string pattern, string value)
        {
            return new WildcardPattern(pattern).IsMatch(value);
        }
    }
}
