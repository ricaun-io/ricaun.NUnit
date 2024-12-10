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
        internal class AppDomainDisposable : IDisposable
        {
            private readonly AppDomain _appDomain;
            public AppDomain AppDomain => _appDomain;
            public AppDomainDisposable(string assemblyPath = null)
            {
                var settings = new AppDomainSetup
                {
                    ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
                };
                _appDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, settings);
            }
            public void Dispose()
            {
                AppDomain.Unload(_appDomain);
            }
        }

        public static AssemblyMetadataAttribute[] GetAssemblyMetadataAttributes(string assemblyPath)
        {
            using (var domain = new AppDomainDisposable())
            {
                var loader = domain.AppDomain.CreateReferenceLoader();
                var result = loader.GetAssemblyMetadataAttributes(assemblyPath);
                return result.Select(e => e.GetAssemblyMetadataAttribute()).ToArray();
            }
        }

        /// <summary>
        /// Get references of the <paramref name="assemblyPath"/> using a different AppDomain
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public static AssemblyName[] GetReferencedAssemblies(string assemblyPath)
        {
            using (var domain = new AppDomainDisposable())
            {
                var loader = domain.AppDomain.CreateReferenceLoader();
                return loader.LoadReferences(assemblyPath);
            }
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

            /// <summary>
            /// Get the metadata attributes of the specified assembly.
            /// </summary>
            /// <param name="assemblyPath">The path of the assembly.</param>
            /// <returns>An array of AssemblyMetadataSerializable objects representing the metadata attributes.</returns>
            /// <remarks><see cref="AssemblyMetadataAttribute"/> can not be use because does not have <see cref="SerializableAttribute"/></remarks>
            public AssemblyMetadataSerializable[] GetAssemblyMetadataAttributes(string assemblyPath)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                return assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                    .Select(AssemblyMetadataSerializable.Create)
                    .ToArray();
            }
        }
    }
#endif
}
