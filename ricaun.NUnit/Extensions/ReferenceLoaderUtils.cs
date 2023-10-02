using System.IO;
using System.Reflection;

namespace ricaun.NUnit.Extensions
{
    /// <summary>
    /// ReferenceLoaderUtils
    /// </summary>
    internal static partial class ReferenceLoaderUtils
    {
#if NETFRAMEWORK
#elif NET
#else
        /// <summary>
        /// GetReferencedAssemblies by Load Assembly with <see cref="File.ReadAllBytes"/>
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public static AssemblyName[] GetReferencedAssemblies(string assemblyPath)
        {
            return GetReferencedAssembliesDefault(assemblyPath);
        }
#endif
        /// <summary>
        /// GetReferencedAssemblies by Load Assembly with <see cref="File.ReadAllBytes"/>
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        internal static AssemblyName[] GetReferencedAssembliesDefault(string assemblyPath)
        {
            var assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
            return assembly.GetReferencedAssemblies();
        }
    }
}