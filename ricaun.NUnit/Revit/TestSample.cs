using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ricaun.NUnit.Revit
{
    public class TestSample
    {
        [Test]
        public void NormalTest()
        {
            var a = 0;
            for (int i = 0; i < 10000000; i++)
            {
                a += i;
            }
            Assert.True(true);
        }

        [Test]
        public void FailTest()
        {
            var a = 0;
            for (int i = 0; i < 10000000; i++)
            {
                a += i;
            }
            Assert.True(false);
        }

        [Test]
        public void ParameterTest(UIApplication uiapp)
        {
            Assert.IsNotNull(uiapp);
        }

        [Test]
        public void SelectTest(UIApplication uiapp)
        {
            Document document = uiapp.ActiveUIDocument.Document;

            var elements = new FilteredElementCollector(document)
                .WhereElementIsNotElementType()
                .OfClass(typeof(Wall))
                .OfType<Wall>();

            Assert.Pass("FilteredElementCollector");
        }
    }
}
