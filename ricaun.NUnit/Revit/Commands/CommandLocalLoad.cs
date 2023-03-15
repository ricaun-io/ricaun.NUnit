using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace ricaun.NUnit.Revit.Commands
{
    [DisplayName("Command - Local Show")]
    [Transaction(TransactionMode.Manual)]
    public class CommandLocalShow : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            var tests = TestEngine.GetTestFullNames(Assembly.GetExecutingAssembly().Location);

            foreach (var test in tests)
            {
                Console.WriteLine(test);
            }

            return Result.Succeeded;
        }
    }

    [DisplayName("Command - Local Load")]
    [Transaction(TransactionMode.Manual)]
    public class CommandLocalLoad : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            var parameters = new object[] { uiapp, uiapp.Application, uiapp.Application.VersionBuild };

            //TestEngineFilter.CancellationTokenTimeOut = TimeSpan.FromSeconds(5);

            var testModel = TestEngine.TestAssembly(Assembly.GetExecutingAssembly().Location, parameters);

            var tests = testModel.Tests.SelectMany(e => e.Tests);

            foreach (var test in tests)
            {
                Console.WriteLine($"\t{test}");
            }

            Console.WriteLine(testModel.Time);

            return Result.Succeeded;
        }
    }

}