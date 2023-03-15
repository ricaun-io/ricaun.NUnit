using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace ricaun.NUnit.Revit.Commands
{
    [DisplayName("Command - Load FileTest")]
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static string LastPath { get; set; } = @"D:\Users\ricau\Documents\GitHub\ricaun-io\ricaun.NUnit\SampleTest.Tests\bin\Debug\SampleTest.Tests.dll";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            LastPath = @"C:\Users\ricau\source\repos\TestProject.Tests\TestProject.Tests\bin\Debug\TestProject.Tests.dll";

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

            var parameters = new object[] { uiapp, uiapp.Application, uiapp.Application.VersionBuild };

            TestUtils.Execute(LastPath, uiapp.Application.VersionNumber, parameters);

            return Result.Succeeded;
        }
    }
}