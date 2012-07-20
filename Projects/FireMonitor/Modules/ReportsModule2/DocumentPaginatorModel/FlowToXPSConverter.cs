using System;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;

namespace ReportsModule2.DocumentPaginatorModel
{
	public class ConvertFlowToXPS
	{
		public ConvertFlowToXPS()
		{

		}

		//public static int SaveAsXps(string fileName)
		//{
		//    object doc;
		//    FileInfo fileInfo = new FileInfo(fileName);
		//    using (FileStream file = fileInfo.OpenRead())
		//    {
		//        System.Windows.Markup.ParserContext context = new System.Windows.Markup.ParserContext();
		//        context.BaseUri = new Uri(fileInfo.FullName, UriKind.Absolute);
		//        doc = System.Windows.Markup.XamlReader.Load(file, context);
		//    }
		//    if (!(doc is IDocumentPaginatorSource))
		//    {
		//        Console.WriteLine("DocumentPaginatorSource expected");
		//        return -1;
		//    }

		//    using (Package container = Package.Open(fileName + ".xps", FileMode.Create))
		//    {
		//        using (XpsDocument xpsDoc = new XpsDocument(container, CompressionOption.Maximum))
		//        {
		//            XpsSerializationManager rsm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
		//            DocumentPaginator paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
		//            paginator = new DocumentPaginatorWrapper(paginator, new Size(768, 676), new Size(48, 48));
		//            rsm.SaveAsXaml(paginator);
		//        }
		//    }
		//    Console.WriteLine("{0} generated.", fileName + ".xps");
		//    return 0;

		//}

		public static XpsDocument CreateXpsDocumentFromString(string xamlFlowDoc)
		{
			object doc;
			doc = XamlReader.Parse(xamlFlowDoc);
			MemoryStream ms = new MemoryStream();
			Package pkg = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);
			string pack = "pack://report.xps";
			PackageStore.RemovePackage(new Uri(pack));
			PackageStore.AddPackage(new Uri(pack), pkg);
			XpsDocument xpsDoc = new XpsDocument(pkg, CompressionOption.NotCompressed, pack);
			XpsSerializationManager rsm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
			DocumentPaginator paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
			//paginator = new PimpedPaginator(paginator,);
			paginator = new DocumentPaginatorWrapper(paginator, new Size(768, 676), new Size(48, 48));
			rsm.SaveAsXaml(paginator);
			return xpsDoc;
		}

		public static void SaveAsXps2(string xamlFlowDoc)
		{
			object doc;
			doc = XamlReader.Parse(xamlFlowDoc);
			
			using (Package container = Package.Open("journal.xps", FileMode.Create))
			{
				using (XpsDocument xpsDoc = new XpsDocument(container, CompressionOption.NotCompressed))
				{
					XpsSerializationManager rsm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
					DocumentPaginator paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
					paginator = new DocumentPaginatorWrapper(paginator, new Size(768, 676), new Size(48, 48));
					rsm.SaveAsXaml(paginator);
				}
			}
		}

		public static void SaveFlowAsXpsInFileDefault(string xamlFlowDoc)
		{
			object flowDocument;
			flowDocument = XamlReader.Parse(xamlFlowDoc);
			using (Package container = Package.Open("journal.xps", FileMode.Create))
			{
				using (XpsDocument xpsDocument = new XpsDocument(container, CompressionOption.Maximum))
				{
					XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
					writer.Write(((IDocumentPaginatorSource)flowDocument).DocumentPaginator);
				}
			}
		}

		public static void SaveFlowAsXpsInFile(FlowDocument flowDocument)
		{
			using (Package container = Package.Open("journal.xps", FileMode.Create))
			{
				using (XpsDocument xpsDoc = new XpsDocument(container, CompressionOption.Maximum))
				{
					XpsSerializationManager rsm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
					DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
					paginator = new DocumentPaginatorWrapper(paginator, new Size(768, 676), new Size(48, 48));
					rsm.SaveAsXaml(paginator);
				}
			}
		}

		public static void SaveFlowAsString(FlowDocument flowDocument)
		{
			var stringFlowDoc = XamlWriter.Save(flowDocument);
			StreamWriter sw = new StreamWriter("StringFlowDoc.txt");
			sw.Write(stringFlowDoc);
			sw.Close();
		}

		public static XpsDocument SaveFlowAsXpsInMemory(FlowDocument flowDocument)
		{
			MemoryStream ms = new MemoryStream(); 
			Package container = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);
			Uri documentUri = new Uri("pack://InMemoryDocument.xps");
			PackageStore.AddPackage(documentUri, container);
			XpsDocument xpsDoc = new XpsDocument(container, CompressionOption.Fast, documentUri.AbsoluteUri);
			XpsSerializationManager rsm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
			DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
			paginator = new DocumentPaginatorWrapper(paginator, new Size(768, 676), new Size(48, 48));
			rsm.SaveAsXaml(paginator);
			xpsDoc.Close();
			container.Close();
			return xpsDoc;
		}
	}
}
