namespace ricaun.NUnit.Extensions
{
#if NETFRAMEWORK
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    /// <summary>
    /// ReferenceLoaderUtils
    /// </summary>
    internal static partial class ReferenceLoaderUtils
    {
        public static AssemblyMetadataAttribute[] GetAssemblyMetadataAttributes(string assemblyPath)
        {
            //var result = new AssemblyMetadataAttribute[] { };

            //var assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
            //result = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToArray();

            var settings = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
            };
            var childDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, settings);

            var loader = childDomain.CreateReferenceLoader();

            //This operation is executed in the new AppDomain
            var result = loader.GetAssemblyMetadataAttributes(assemblyPath);

            AppDomain.Unload(childDomain);

            return result.Select(e=>e.GetAssemblyMetadataAttribute()).ToArray();
        }

        /// <summary>
        /// Get references of the <paramref name="assemblyPath"/> using a different AppDomain
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public static AssemblyName[] GetReferencedAssemblies(string assemblyPath)
        {
            var settings = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
            };
            var childDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, settings);

            var loader = childDomain.CreateReferenceLoader();

            //This operation is executed in the new AppDomain
            var assemblyNames = loader.LoadReferences(assemblyPath);

            AppDomain.Unload(childDomain);

            return assemblyNames;
        }

        private static ReferenceLoader CreateReferenceLoader(this AppDomain domain)
        {
            return domain.CreateInstanceAndUnwrap<ReferenceLoader>();
        }

        private static T CreateInstanceAndUnwrap<T>(this AppDomain domain, params object[] args) where T : MarshalByRefObject
        {
            try
            {
                var handle = Activator.CreateInstance(domain,
                           typeof(T).Assembly.FullName,
                           typeof(T).FullName,
                           false, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, args, CultureInfo.CurrentCulture, new object[0]);

                return (T)handle.Unwrap();
            }
            catch { }

            try
            {
                var handle = domain.CreateInstanceFrom(
                        typeof(T).Assembly.Location,
                        typeof(T).FullName,
                        false, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, args, CultureInfo.CurrentCulture, new object[0]);

                return (T)handle.Unwrap();
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException($"CreateInstanceFromAndUnwrap fail to CreateInstance<{typeof(T).Name}>, Location: {typeof(T).Assembly.Location}", ex);
            }
        }

        /// <summary>
        /// ReferenceLoader
        /// <code>
        /// https://stackoverflow.com/questions/225330/how-to-load-a-net-assembly-for-reflection-operations-and-subsequently-unload-it/37970043#37970043
        /// </code>
        /// </summary>
        private class ReferenceLoader : MarshalByRefObject
        {
            /// <summary>
            /// LoadReferences
            /// </summary>
            /// <param name="assemblyPath"></param>
            /// <returns></returns>
            public AssemblyName[] LoadReferences(string assemblyPath)
            {
                var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                var assemblyNames = assembly.GetReferencedAssemblies().ToArray();
                return assemblyNames;
            }

            public AssemblyMetadataSerializable[] GetAssemblyMetadataAttributes(string assemblyPath)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                return assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                    .Select(e => new AssemblyMetadataSerializable(e))
                    .ToArray();
            }
        }

        [Serializable]
        public class AssemblyMetadataSerializable
        {
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
#endif
}
