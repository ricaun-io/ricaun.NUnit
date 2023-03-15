using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.ComponentModel;

namespace ricaun.NUnit.Revit.Commands
{
    [DisplayName("Command - SampleTest")]
    [Transaction(TransactionMode.Manual)]
    public class CommandCopyDll : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            var dllPath = @"D:\Users\ricau\Documents\GitHub\ricaun-io\ricaun.NUnit\SampleTest.Tests\bin\Debug\SampleTest.Tests.dll";

            TestUtils.Execute(dllPath);

            return Result.Succeeded;
        }
    }
}