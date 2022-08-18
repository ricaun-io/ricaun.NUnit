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

            var nunits = AppDomain.CurrentDomain.GetAssemblies().Where(e => e.GetName().Name.Equals("nunit.framework"));
            foreach (var item in nunits)
            {
                Console.WriteLine($"{item} \t{item.Location}");
            }

            //Console.WriteLine(TestEngine.Version);

            var location = Assembly.GetExecutingAssembly().Location;
            var test = TestEngine.TestAssembly(location, uiapp);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(test);
            Clipboard.SetText(json);
            Console.WriteLine(test);

            return Result.Succeeded;
        }
    }
}
#endif