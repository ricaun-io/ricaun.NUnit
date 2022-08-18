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

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Console.WriteLine(TestEngine.Version);

            var location = Assembly.GetExecutingAssembly().Location;
            var test = TestEngine.TestAssembly(location, uiapp);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(test);
            Clipboard.SetText(json);
            Console.WriteLine(test);

            //System.Windows.MessageBox.Show(test.ToString());

            //var choofdlog = new Microsoft.Win32.OpenFileDialog();
            //choofdlog.Filter = "All Files (*.dll)|*.dll";
            //choofdlog.FilterIndex = 1;

            //if (choofdlog.ShowDialog() == true)
            //{
            //    var fileName = choofdlog.FileName;
            //    Console.WriteLine(fileName);
            //    var test = TestEngine.TestAssembly(fileName);
            //    Console.WriteLine(test);
            //    Console.WriteLine(test.Message);
            //}

            var location2 = @"C:\Users\ricau\Downloads\Revit.TestRunner.SampleTestProject2.dll";
            var test2 = TestEngine.TestAssembly(location2);
            Console.WriteLine(test2);
            Console.WriteLine(test2.Message);

            var location3 = @"C:\Users\ricau\source\repos\RevitAddin.UnitTest\RevitAddin.UnitTest\bin\Release\2017\RevitAddin.UnitTest.dll";
            var test3 = TestEngine.TestAssembly(location3);
            Console.WriteLine(test3);
            Console.WriteLine(test3.Message);

            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;

            return Result.Succeeded;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine($"AssemblyResolve {args.Name}");
            return null;
        }
    }
}
#endif