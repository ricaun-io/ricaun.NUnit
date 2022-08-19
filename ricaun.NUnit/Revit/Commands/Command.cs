#if DEBUG
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace ricaun.NUnit.Revit.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            //var nunits = AppDomain.CurrentDomain.GetAssemblies()
            //    .Where(e => e.GetName().Name.Equals("nunit.framework"));

            //foreach (var item in nunits)
            //{
            //    Console.WriteLine($"{item} \t{item.Location}");
            //}

            //Console.WriteLine(TestEngine.Version);

            var location = Assembly.GetExecutingAssembly().Location;
            var test = TestEngine.TestAssembly(location, "This is a custom string parameter.", uiapp);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(test);
            Clipboard.SetText(json);
            Console.WriteLine(test);

            //Console.WriteLine(test.Console);
            //foreach (var testClass in test.Tests)
            //{
            //    Console.WriteLine($"{testClass}\t{testClass.Console}");
            //    foreach (var testMethod in testClass.Tests)
            //    {
            //        Console.WriteLine($"{testMethod}\t{testMethod.Console}");
            //    }
            //}

            return Result.Succeeded;
        }
    }
}
#endif