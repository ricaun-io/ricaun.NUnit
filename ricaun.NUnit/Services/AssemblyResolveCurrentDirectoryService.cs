using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// AssemblyResolve and CurrentDirectory
    /// </summary>
    internal class AssemblyResolveCurrentDirectoryService : CurrentDirectory, IDisposable
    {
        private AssemblyResolveService assemblyResolveService;

        /// <summary>
        /// AssemblyResolveCurrentDirectoryService
        /// </summary>
        /// <param name="directory"></param>
        public AssemblyResolveCurrentDirectoryService(string directory) : base(directory)
        {
            this.assemblyResolveService = new AssemblyResolveService(directory);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public override void Dispose()
        {
            this.assemblyResolveService.Dispose();
            base.Dispose();
        }
    }
}
