#if DEBUG
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace ricaun.NUnit.Revit.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            var location = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(location);

            //foreach (var item in Directory.GetFiles(Path.GetDirectoryName(location), "*.dll"))
            //{
            //    Console.WriteLine($"{TestEngine.ContainNUnit(item)} {Path.GetFileName(item)}");
            //}
            //var test = TestEngine.TestAssembly(location, "This is a custom string parameter.", uiapp);
            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(test);
            //System.Windows.Clipboard.SetText(json);
            //Console.WriteLine(test);


            UnZipAndTestFiles(uiapp.Application, directory);





            return Result.Succeeded;
        }


        private static void CopyFile(string filePath, string directory)
        {
            File.Copy(filePath, Path.Combine(directory, Path.GetFileName(filePath)), true);
        }

        private static void UnZipAndTestFiles(Application application, string directory)
        {
            var versionNumber = application.VersionNumber;

            if (Directory.GetFiles(directory, "*.zip").FirstOrDefault() is string zipFile)
            {
                if (ZipExtension.ExtractToFolder(zipFile, out string zipDestination))
                {
                    foreach (var versionDirectory in Directory.GetDirectories(zipDestination))
                    {
                        if (Path.GetFileName(versionDirectory).Equals(versionNumber))
                        {
                            Console.WriteLine($"Test VersionNumber: {versionNumber}");
                            TestDirectory(application, versionDirectory);
                        }
                    }

                    TestDirectory(application, zipDestination);
                }
            }
        }

        private static void TestDirectory(Application application, string directory)
        {
            foreach (var filePath in Directory.GetFiles(directory, "*.dll"))
            {
                var fileName = Path.GetFileName(filePath);
                Console.WriteLine($"Test File: {fileName}");
                try
                {
                    if (TestEngine.ContainNUnit(filePath))
                    {
                        var modelTest = TestEngine.TestAssembly(
                            filePath,
                            application,
                            application.GetControlledApplication());

                        System.Windows.Clipboard.SetText(Newtonsoft.Json.JsonConvert.SerializeObject(modelTest));

                        Console.WriteLine(modelTest);
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine($"Error: {fileName}");
                }
            }
        }
    }

    public static class ZipExtension
    {
        /// <summary>
        /// Get <see cref="ControlledApplication"/> using the <paramref name="application"/>
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public static ControlledApplication GetControlledApplication(this Application application)
        {
            var type = typeof(ControlledApplication);

            var constructor = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { application.GetType() }, null);

            return constructor?.Invoke(new object[] { application }) as ControlledApplication;
        }

        /// <summary>
        /// Extract <paramref name="zipFile"/> to Folder
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
                return true;

            Directory.CreateDirectory(zipDestination);

            try
            {
                ZipFile.ExtractToDirectory(zipFile, zipDestination);
                return true;
            }
            catch { }

            return false;
        }
    }
}
#endif