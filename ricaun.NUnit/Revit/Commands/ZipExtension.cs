using System;
using System.IO;
using System.IO.Compression;

namespace ricaun.NUnit.Revit.Commands
{
    public static class ZipExtension
    {
        /// <summary>
        /// Extract <paramref name="zipFile"/> to Folder add temp folder if exist
        /// </summary>
        /// <param name="zipFile"></param>
        /// <param name="zipDestination"></param>
        /// <returns></returns>
        public static bool ExtractToFolder(string zipFile, out string zipDestination)
        {
            var zipName = Path.GetFileNameWithoutExtension(zipFile);
            var zipDirectory = Path.GetDirectoryName(zipFile);
            zipDestination = Path.Combine(zipDirectory, zipName);

            if (Directory.Exists(zipDestination))
            {
                try
                {
                    Directory.Delete(zipDestination, true);
                }
                catch { }
            }

            if (Directory.Exists(zipDestination))
            {
                zipDestination += $"_{DateTime.Now.Ticks}";
            }

            Directory.CreateDirectory(zipDestination);

            try
            {
                ZipFile.ExtractToDirectory(zipFile, zipDestination);
                return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Creates a zip archive that contains the files and directories from the specified directory.
        /// </summary>
        /// <param name="sourceDirectoryName"></param>
        /// <param name="destinationArchiveFileName"></param>
        /// <param name="includeBaseDirectory"></param>
        public static string CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, bool includeBaseDirectory = false)
        {
            destinationArchiveFileName = Path.ChangeExtension(destinationArchiveFileName, "zip");
            var folder = Path.GetDirectoryName(destinationArchiveFileName);
            if (Directory.Exists(folder) == false) Directory.CreateDirectory(folder);
            if (File.Exists(destinationArchiveFileName)) return destinationArchiveFileName;
            ZipFile.CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, CompressionLevel.Optimal, includeBaseDirectory);
            return destinationArchiveFileName;
        }
    }
}