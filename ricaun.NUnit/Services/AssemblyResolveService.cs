using System;
using System.Globalization;
using System.Reflection;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// AssemblyResolve and CurrentDirectory
    /// </summary>
    public class AssemblyResolveService : CurrentDirectory, IDisposable
    {
        /// <summary>
        /// AssemblyResolveService
        /// </summary>
        /// <param name="directory"></param>
        public AssemblyResolveService(string directory) : base(directory)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var requestedAssemblyName = new AssemblyName(args.Name);

            Assembly assembly = ReadExistingAssembly(requestedAssemblyName);

            return assembly;
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
        public override void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            base.Dispose();
        }
    }
}
