namespace ricaun.NUnit.Extensions
{
#if NET
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Loader;

    /// <summary>
    /// ReferenceLoaderUtils
    /// </summary>
    internal static partial class ReferenceLoaderUtils
    {
        /// <summary>
        /// GetAssemblyMetadataAttributes by Load Assembly with <see cref="ReferenceLoaderUtils.SimpleLoadContextUtil"/>
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public static AssemblyMetadataAttribute[] GetAssemblyMetadataAttributes(string assemblyPath)
        {
            var result = new AssemblyMetadataSerializable[] { };
            SimpleLoadContextUtil.LoadAndUnload(assemblyPath, (assembly) =>
            {
                result = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                    .Select(AssemblyMetadataSerializable.Create)
                    .ToArray();
            });

            return result.Select(e=>e.GetAssemblyMetadataAttribute()).ToArray();
        }

        /// <summary>
        /// GetReferencedAssemblies by Load Assembly with <see cref="ReferenceLoaderUtils.SimpleLoadContextUtil"/>
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public static AssemblyName[] GetReferencedAssemblies(string assemblyPath)
        {
            var result = new AssemblyName[] { };
            SimpleLoadContextUtil.LoadAndUnload(assemblyPath, (assembly) =>
            {
                result = assembly.GetReferencedAssemblies().ToArray();
            });

            return result;
        }

        /// <summary>
        /// SimpleLoadContextUtil
        /// </summary>
        public class SimpleLoadContextUtil
        {
            private const int RepeatGC = 10;

            /// <summary>
            /// LoadContextWeakReference
            /// </summary>
            internal static WeakReference LoadContextWeakReference;

            /// <summary>
            /// Load and unload assembly
            /// </summary>
            /// <param name="assemblyPath"></param>
            /// <param name="assemblyPluginAction"></param>
            /// <returns>WeekReference IsAlive</returns>
            public static bool LoadAndUnload(string assemblyPath, Action<Assembly> assemblyPluginAction = null)
            {
                ExecuteAndUnload(assemblyPath, out LoadContextWeakReference, assemblyPluginAction);

                for (int i = 0; LoadContextWeakReference.IsAlive && (i < RepeatGC); i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                return LoadContextWeakReference.IsAlive;
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            internal static void ExecuteAndUnload(string assemblyPath, out WeakReference alcWeakRef, Action<Assembly> assemblyPluginAction = null)
            {
                var alc = new SimpleLoadContext(assemblyPath);
                Assembly assemblyPlugin = alc.LoadFromAssemblyPath(assemblyPath);

                assemblyPluginAction?.Invoke(assemblyPlugin);

                alcWeakRef = new WeakReference(alc, trackResurrection: true);
                alc.Unload();
            }
        }

        /// <summary>
        /// SimpleLoadContext
        /// </summary>
        /// <remarks>https://learn.microsoft.com/en-us/dotnet/standard/assembly/unloadability</remarks>
        internal class SimpleLoadContext : AssemblyLoadContext
        {
            private AssemblyDependencyResolver _resolver;

            public SimpleLoadContext(string mainAssemblyToLoadPath) : base(isCollectible: true)
            {
                _resolver = new AssemblyDependencyResolver(mainAssemblyToLoadPath);
            }

            protected override Assembly Load(AssemblyName name)
            {
                string assemblyPath = _resolver.ResolveAssemblyToPath(name);
                if (assemblyPath != null)
                {
                    return LoadFromAssemblyPath(assemblyPath);
                }

                return null;
            }
        }
    }
#endif
}