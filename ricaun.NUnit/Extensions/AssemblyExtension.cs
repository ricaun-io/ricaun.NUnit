using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ricaun.NUnit.Extensions
{
    /// <summary>
    /// AssemblyExtension
    /// </summary>
    internal static class AssemblyExtension
    {
        /// <summary>
        /// LoadReferencedAssemblies
        /// </summary>
        /// <param name="assembly"></param>
        public static void LoadReferencedAssemblies(this Assembly assembly)
        {
            foreach (var assemblyReference in assembly.GetReferencedAssemblies())
            {
                if (TryLoadAssemblyName(assemblyReference) is null)
                {
                    LoadAssemblyReference(assembly, assemblyReference);
                }
            }
        }

        /// <summary>
        /// Try to Load <paramref name="assemblyName"/>
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private static Assembly TryLoadAssemblyName(AssemblyName assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch { }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return assemblies
                .FirstOrDefault(e => e.GetName().FullName.Equals(assemblyName.FullName));

        }

        private static Assembly LoadAssemblyReference(Assembly assembly, AssemblyName assemblyName)
        {
            string location = assembly.Location;
            string path = Path.GetDirectoryName(location);
            var files = Directory.GetFiles(path, "*.dll");

            var fileReference = files
                        .FirstOrDefault(e => AssemblyName.GetAssemblyName(e).FullName == assemblyName.FullName);

            if (fileReference is not null)
            {
                try
                {
                    Assembly aReference = Assembly.LoadFile(fileReference);
                    Debug.WriteLine($"Load Reference: {aReference}");
                    return aReference;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Load Reference Exception {ex}");
                }
            }

            return null;
        }
    }
}
