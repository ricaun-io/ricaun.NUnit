using Autodesk.Revit.UI;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace ricaun.NUnit.Revit.Commands
{
    public static class TestUtils
    {
        public static void Execute(string filePath, string versionNumber = "2021", params object[] parameters)
        {
            if (filePath is null) return;

            var location = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(location);

            string copyPath = null;
            if (Path.GetExtension(filePath).EndsWith("zip"))
            {
                copyPath = CopyFile(filePath, directory);
            }
            else if (Path.GetExtension(filePath).EndsWith("dll"))
            {
                copyPath = ZipExtension.CreateFromDirectory(
                    Path.GetDirectoryName(filePath),
                    Path.Combine(directory, Path.GetFileName(filePath))
                    );
            }
            else return;

            //if (System.Windows.Input.Keyboard.Modifiers == ModifierKeys.Control)
            //    Process.Start(directory);

            UnZipAndTestFiles(directory, versionNumber, parameters);

            if (copyPath is not null)
                File.Delete(copyPath);
        }

        private static string CopyFile(string filePath, string directory)
        {
            var copy = Path.Combine(directory, Path.GetFileName(filePath));
            File.Copy(filePath, copy, true);
            return copy;
        }

        private static void UnZipAndTestFiles(string directory, string versionNumber, params object[] parameters)
        {
            if (Directory.GetFiles(directory, "*.zip").FirstOrDefault() is string zipFile)
            {
                if (ZipExtension.ExtractToFolder(zipFile, out string zipDestination))
                {
                    foreach (var versionDirectory in Directory.GetDirectories(zipDestination))
                    {
                        if (Path.GetFileName(versionDirectory).Equals(versionNumber))
                        {
                            Console.WriteLine($"Test VersionNumber: {versionNumber}");
                            TestDirectory(versionDirectory, parameters);
                        }
                    }

                    TestDirectory(zipDestination, parameters);
                }
            }
        }

        private static void TestDirectory(string directory, params object[] parameters)
        {
            Console.WriteLine($"NUnit: {typeof(NUnitAttribute).Assembly.GetName().Version}");
            Console.WriteLine($"TestEngine.Version: {TestEngine.Version}");
            Console.WriteLine($"TestEngine.VersionNUnit: {TestEngine.VersionNUnit}");
            foreach (var filePath in Directory.GetFiles(directory, "*.dll"))
            {
                var fileName = Path.GetFileName(filePath);
                try
                {
                    if (TestEngine.ContainNUnit(filePath))
                    {
                        Console.WriteLine($"Test File: {fileName}");
                        //ValidateTestAssemblyNUnitVersion(filePath);
                        //foreach (var testName in TestEngine.GetTestFullNames(filePath))
                        //{
                        //    Console.WriteLine(testName);
                        //}

                        var modelTest = TestEngine.TestAssembly(
                            filePath, parameters);

                        //System.Windows.Clipboard.SetText(Newtonsoft.Json.JsonConvert.SerializeObject(modelTest));
                        //System.Windows.Clipboard.SetText(modelTest.AsString());

                        Console.WriteLine($"\t{modelTest}");
                        Console.WriteLine(modelTest.Message);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {fileName} {ex}");
                }
            }
        }


        private static void ValidateTestAssemblyNUnitVersion(string location)
        {
            const string NUNIT_ASSEMBLY = "nunit.framework";
            string reference = NUNIT_ASSEMBLY;
            {
                var fileReference = Directory.GetFiles(Path.GetDirectoryName(location), $"{reference}.dll")
                    .FirstOrDefault();

                if (fileReference is not null)
                {
                    {
                        var assemblyLoad = Assembly.LoadFile(fileReference);
                        Console.WriteLine($"Assembly.LoadFile({assemblyLoad}) {assemblyLoad.Location}");
                        return;
                    }
                }
            }
        }
    }
}
