using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NUnit.Framework;
using System;
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

            Assert.Pass($"FilteredElementCollector {elements.Count()}");
        }

        [Test]
        public void UIApplicationEvent(UIApplication uiapp)
        {
            try
            {
                uiapp.Idling += Uiapp_Idling;
                uiapp.Idling -= Uiapp_Idling;
            }
            catch (Exception)
            {
                throw;
            }
            Assert.Pass("Idling");
        }

        private static void Uiapp_Idling(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e) { }

        //[Test]
        //public void DocumentsTransaction(UIApplication uiapp)
        //{
        //    var userName = uiapp.Application.Username;
        //    var documents = uiapp.Application.Documents.OfType<Document>();
        //    foreach (var document in documents)
        //    {
        //        if (document.IsLinked) continue;
        //        using (Transaction transaction = new Transaction(document))
        //        {
        //            transaction.Start("Change to Username");
        //            document.ProjectInformation.Author = userName;
        //            transaction.Commit();
        //        }
        //        Console.WriteLine($"{document.Title} {document.ProjectInformation.Author}");
        //        Assert.AreEqual(userName, document.ProjectInformation.Author);
        //    }
        //}
    }
}
