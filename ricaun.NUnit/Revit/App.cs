#if DEBUG
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ricaun.Revit.UI;

namespace ricaun.NUnit.Revit
{

    [AppLoader]
    public class App : IExternalApplication
    {
        private static RibbonPanel ribbonPanel;
        public Result OnStartup(UIControlledApplication application)
        {
            ribbonPanel = application.CreatePanel("ricaun.NUnit");
            ribbonPanel.CreatePushButton<Commands.Command>()
                .SetLargeImage(Properties.Resources.ricaun.GetBitmapSource());
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            ribbonPanel?.Remove();
            return Result.Succeeded;
        }
    }
}
#endif