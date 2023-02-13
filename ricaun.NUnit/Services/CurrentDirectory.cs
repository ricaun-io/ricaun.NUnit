using System;
using System.IO;
using System.Reflection;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// CurrentDirectory
    /// </summary>
    internal class CurrentDirectory : IDisposable
    {
        private readonly string currentDirectory;
        private string CallingAssemblyDirectory => Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

        /// <summary>
        /// CurrentDirectory
        /// </summary>
        public CurrentDirectory()
        {
            this.currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(CallingAssemblyDirectory);
        }
        /// <summary>
        /// CurrentDirectory
        /// </summary>
        /// <param name="directory"></param>
        public CurrentDirectory(string directory)
        {
            this.currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(directory);
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            Directory.SetCurrentDirectory(this.currentDirectory);
        }
    }

}