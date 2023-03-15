using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NUnit.Framework;
using System.Linq;

namespace ricaun.NUnit.Revit
{
    public class TestRevitSample
    {
        [Test]
        public void ParameterTest(UIApplication uiapp)
        {
            Assert.IsNotNull(uiapp);
        }

        [Test]
        public void SelectTest(UIApplication uiapp)
        {
            Document document = uiapp.ActiveUIDocument.Document;

            if (document is null)
                Assert.Ignore("Document is null.");

            var elements = new FilteredElementCollector(document)
                .WhereElementIsNotElementType()
                .OfClass(typeof(Wall))
                .OfType<Wall>();

            Assert.Pass("FilteredElementCollector");
        }
    }
}
