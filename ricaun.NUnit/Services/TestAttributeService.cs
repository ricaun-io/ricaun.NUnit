using NUnit.Framework;
using System.Reflection;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// TestAttributeService
    /// </summary>
    public class TestAttributeService : AttributeService
    {
        /// <summary>
        /// AnyTestAttribute
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <returns></returns>
        public bool AnyTestAttribute(ICustomAttributeProvider customAttributeProvider)
        {
            return AnyAttributeName<TestAttribute>(customAttributeProvider) || AnyAttributeName<TestCaseAttribute>(customAttributeProvider);
        }
    }
}
