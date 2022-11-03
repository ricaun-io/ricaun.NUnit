using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace ricaun.NUnit.Revit.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static string LastPath { get; set; } = @"D:\Users\ricau\Documents\GitHub\ricaun-io\ricaun.NUnit\SampleTest.Tests\bin\Debug\SampleTest.Tests.dll";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            if (!File.Exists(LastPath)) LastPath = null;

            if (LastPath is null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == true)
                {
                    //Get the path of specified file
                    LastPath = openFileDialog.FileName;
                }
            }

            Execute(uiapp, LastPath);


            return Result.Succeeded;
        }

        private static void Execute(UIApplication uiapp, string filePath)
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

            if (System.Windows.Input.Keyboard.Modifiers == ModifierKeys.Control)
                Process.Start(directory);

            UnZipAndTestFiles(uiapp, directory);

            if (copyPath is not null)
                File.Delete(copyPath);
        }

        private static string CopyFile(string filePath, string directory)
        {
            var copy = Path.Combine(directory, Path.GetFileName(filePath));
            File.Copy(filePath, copy, true);
            return copy;
        }

        private static void UnZipAndTestFiles(UIApplication uiapp, string directory)
        {
            var versionNumber = uiapp.Application.VersionNumber;

            if (Directory.GetFiles(directory, "*.zip").FirstOrDefault() is string zipFile)
            {
                if (ZipExtension.ExtractToFolder(zipFile, out string zipDestination))
                {
                    foreach (var versionDirectory in Directory.GetDirectories(zipDestination))
                    {
                        if (Path.GetFileName(versionDirectory).Equals(versionNumber))
                        {
                            Console.WriteLine($"Test VersionNumber: {versionNumber}");
                            TestDirectory(uiapp, versionDirectory);
                        }
                    }

                    TestDirectory(uiapp, zipDestination);
                }
            }
        }

        private static void TestDirectory(UIApplication uiapp, string directory)
        {
            var application = uiapp.Application;
            foreach (var filePath in Directory.GetFiles(directory, "*.dll"))
            {
                var fileName = Path.GetFileName(filePath);
                try
                {
                    if (TestEngine.ContainNUnit(filePath))
                    {
                        Console.WriteLine($"Test File: {fileName}");

                        var modelTest = TestEngine.TestAssembly(
                            filePath,
                            application,
                            application.GetControlledApplication(),
                            uiapp);

                        System.Windows.Clipboard.SetText(Newtonsoft.Json.JsonConvert.SerializeObject(modelTest));

                        Console.WriteLine($"\t{modelTest}");
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine($"Error: {fileName}");
                }
            }
        }
    }
}