using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.IO.Packaging;
using System.Drawing.Printing;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;

namespace Infrastructure.Client.Converters
{
    public static class WMFConverter
    {
        public static Canvas ReadCanvas(string fileName)
        {
            var temp = Path.GetTempFileName();
            using (PrintDocument pdx = new PrintDocument())
            {
                pdx.PrintPage += (object printSender, PrintPageEventArgs printE) =>
                {
                    var img = System.Drawing.Image.FromFile(fileName);
                    printE.Graphics.DrawImageUnscaled(img, printE.PageSettings.Bounds);
                    printE.HasMorePages = false;
                };
                pdx.DefaultPageSettings.Landscape = true;
                pdx.PrinterSettings.PrinterName = "Microsoft XPS Document Writer";
                pdx.PrinterSettings.PrintToFile = true;
                pdx.PrinterSettings.PrintFileName = temp;
                pdx.PrintController = new StandardPrintController();
                pdx.Print();
            }
            GC.Collect();

            Canvas canvas;
            using (Package package = Package.Open(temp, FileMode.Open, FileAccess.Read))
            {
                string inMemoryPackageName = "memorystream://miXps.xps";
                Uri packageUri = new Uri(inMemoryPackageName);
                PackageStore.AddPackage(packageUri, package);
                using (XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.NotCompressed, inMemoryPackageName))
                {
                    FixedDocumentSequence fixedDocumentSequence = xpsDocument.GetFixedDocumentSequence();
                    DocumentReference docReference = xpsDocument.GetFixedDocumentSequence().References.First();
                    FixedDocument doc = docReference.GetDocument(false);
                    PageContent content = doc.Pages[0];
                    var fixedPage = content.GetPageRoot(false);
                    canvas = new Canvas()
                    {
                        Width = fixedPage.Width,
                        Height = fixedPage.Height,
                    };
                    for (int i = fixedPage.Children.Count - 2; i >= 0; i--)
                    {
                        var child = fixedPage.Children[i];
                        fixedPage.Children.Remove(child);
                        canvas.Children.Insert(0, child);
                    }
                    xpsDocument.Close();
                }
                package.Close();
                PackageStore.RemovePackage(packageUri);
            }
            GC.Collect();
            try
            {
                File.Delete(temp);
            }
            catch
            {
            }
            return canvas;
        }
    }
}
