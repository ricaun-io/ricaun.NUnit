using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// AssemblyResolveService
    /// </summary>
    public class AssemblyResolveService : IDisposable
    {
        private readonly string directory;
        private readonly bool includeSubDirectories;

        /// <summary>
        /// AssemblyResolveService
        /// </summary>
        /// <param name="directory">Directory to find the Assembly to be loaded.</param>
        /// <param name="includeSubDirectories">Include in the Search in subdirectories.</param>
        public AssemblyResolveService(string directory, bool includeSubDirectories = false)
        {
            this.directory = directory;
            this.includeSubDirectories = includeSubDirectories;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var requestedAssemblyName = new AssemblyName(args.Name);

            Debug.WriteLine($"AssemblyResolve: [Resolve] {requestedAssemblyName}");

            Assembly assembly = ReadExistingAssembly(requestedAssemblyName);

            if (assembly is null)
                assembly = LoadAssemblyWithPattern(requestedAssemblyName, requestedAssemblyName.Name);

            if (assembly is null)
                assembly = LoadAssemblyWithPattern(requestedAssemblyName);

            return assembly;
        }

        private Assembly LoadAssemblyWithPattern(AssemblyName name, string pattern = "*")
        {
            if (string.IsNullOrWhiteSpace(directory))
                return null;

            var searchOption = includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var assemblyFile in Directory.GetFiles(directory, $"{pattern}.dll", searchOption))
            {
                try
                {
                    var assemblyName = AssemblyName.GetAssemblyName(assemblyFile);
                    if (name.ToString() == assemblyName.ToString())
                    {
                        Debug.WriteLine($"AssemblyResolve: [Load] {assemblyName}");
                        return Assembly.LoadFile(assemblyFile);
                    }
                }
                catch { }
            }
            return null;
        }

        private Assembly ReadExistingAssembly(AssemblyName name)
        {
            var currentDomain = AppDomain.CurrentDomain;
            var assemblies = currentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var currentName = assembly.GetName();
                if (string.Equals(currentName.Name, name.Name, StringComparison.InvariantCultureIgnoreCase) &&
                    string.Equals(CultureToString(currentName.CultureInfo), CultureToString(name.CultureInfo), StringComparison.InvariantCultureIgnoreCase))
                {
                    Debug.WriteLine($"AssemblyResolve: [Loaded] {name}");
                    return assembly;
                }
            }
            return null;
        }

        private string CultureToString(CultureInfo culture)
        {
            if (culture == null) return "";
            return culture.Name;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }
    }

}
