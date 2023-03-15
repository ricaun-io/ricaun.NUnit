using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ricaun.NUnit.Extensions;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace ricaun.NUnit.Revit.Commands
{
    [DisplayName("Command - Local Task")]
    [Transaction(TransactionMode.Manual)]
    public class CommandLocalTask : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            var instance = Activator.CreateInstance<TestSampleClass>();
            var method = instance.GetType().GetMethod("TaskTest");
            var methodReturn = method.ReturnParameter.ParameterType;
            //Console.WriteLine(instance);
            Console.WriteLine(method);

            if (methodReturn == typeof(Task))
            {
                TaskSTA.Run(() =>
                {
                    var invoke = method.Invoke(instance, null);
                    if (invoke is Task taskInvoke)
                        taskInvoke.GetAwaiter().GetResult();

                }).Wait(1000);

                TaskSTA.RunSafe(() =>
                {
                    var invoke = method.Invoke(instance, null);
                    if (invoke is Task taskInvoke)
                        taskInvoke.GetAwaiter().GetResult();

                });

                var i = TaskSTA.RunSafe(() =>
                {
                    var invoke = method.Invoke(instance, null);
                    if (invoke is Task taskInvoke)
                        taskInvoke.GetAwaiter().GetResult();
                    return invoke;
                });
                Console.WriteLine(i);

                TaskSTA.Run(() =>
                {
                    Console.WriteLine(".");
                    Thread.Sleep(100);
                    Console.WriteLine("..");
                    Thread.Sleep(100);
                    Console.WriteLine("...");

                }).Wait(1000);

                Console.WriteLine($"Finish {methodReturn}");
            }

            return Result.Succeeded;
        }
    }

}