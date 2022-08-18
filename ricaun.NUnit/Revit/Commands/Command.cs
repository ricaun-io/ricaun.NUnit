#if DEBUG
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
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

            var location = Assembly.GetExecutingAssembly().Location;
            var test = TestEngine.TestAssembly(location, uiapp);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(test);

            Clipboard.SetText(json);

            System.Windows.MessageBox.Show(test.ToString());

            return Result.Succeeded;
        }
    }
}
#endif