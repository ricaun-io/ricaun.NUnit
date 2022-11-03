using Autodesk.Revit.ApplicationServices;
using System;
using System.Reflection;

namespace ricaun.NUnit.Revit.Commands
{
    public static class ControlledApplicationExtension
    {
        /// <summary>
        /// Get <see cref="ControlledApplication"/> using the <paramref name="application"/>
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public static ControlledApplication GetControlledApplication(this Application application)
        {
            var type = typeof(ControlledApplication);

            var constructor = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { application.GetType() }, null);

            return constructor?.Invoke(new object[] { application }) as ControlledApplication;
        }
    }
}