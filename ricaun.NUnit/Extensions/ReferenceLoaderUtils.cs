using System;
using System.IO;
using System.Linq;
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

#if NETFRAMEWORK
#elif NET
#else
        /// <summary>
        /// GetAssemblyMetadataAttributes by Load Assembly with <see cref="File.ReadAllBytes"/>
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public static AssemblyMetadataAttribute[] GetAssemblyMetadataAttributes(string assemblyPath)
        {
            return GetAssemblyMetadataAttributesDefault(assemblyPath);
        }
#endif

        /// <summary>
        /// GetAssemblyMetadataAttributesDefault by Load Assembly with <see cref="File.ReadAllBytes"/>
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        internal static AssemblyMetadataAttribute[] GetAssemblyMetadataAttributesDefault(string assemblyPath)
        {
            var assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
            return assembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToArray();
        }

        [Serializable]
        public class AssemblyMetadataSerializable
        {
            public static AssemblyMetadataSerializable Create(AssemblyMetadataAttribute assemblyMetadataAttribute)
            {
                return new AssemblyMetadataSerializable(assemblyMetadataAttribute);
            }
            public AssemblyMetadataSerializable(AssemblyMetadataAttribute assemblyMetadataAttribute)
            {
                Key = assemblyMetadataAttribute.Key;
                Value = assemblyMetadataAttribute.Value;
            }
            public AssemblyMetadataAttribute GetAssemblyMetadataAttribute()
            {
                return new AssemblyMetadataAttribute(Key, Value);
            }
            public string Key { get; }
            public string Value { get; }
        }
    }
}