using NUnit.Framework;
using System.Reflection;

namespace ricaun.NUnit.Extensions
{
    /// <summary>
    /// NUnitAttributeExtension
    /// </summary>
    internal static class NUnitAttributeExtension
    {
        private const string REASON_FIELD = "_reason";

        /// <summary>
        /// GetReason
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string GetReason<T>(this T attribute) where T : NUnitAttribute
        {
            var fieldInfo = typeof(T)
                .GetField(REASON_FIELD, BindingFlags.Instance | BindingFlags.NonPublic);

            return (string)fieldInfo?.GetValue(attribute);
        }
    }
}
