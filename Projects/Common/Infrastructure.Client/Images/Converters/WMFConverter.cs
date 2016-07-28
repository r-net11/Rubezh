using System;
using Common;
using System.Drawing.Printing;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using Infrastructure.Client.Images;

namespace Infrastructure.Client.Converters
{
	public static class WMFConverter
	{
		public static WMFImage ReadWMF(string fileName)
		{
			var temp = Path.GetTempFileName();
			using (var pdx = new PrintDocument())
			{
				pdx.PrintPage += (printSender, printE) =>
				{
					var img = System.Drawing.Image.FromFile(fileName);
					printE.Graphics.DrawImageUnscaled(img, printE.PageSettings.Bounds);
					printE.HasMorePages = false;
				};
				pdx.DefaultPageSettings.PaperSize = new PaperSize("", 3500, 8000);
				pdx.DefaultPageSettings.Landscape = true;
				pdx.PrinterSettings.PrinterName = "Microsoft XPS Document Writer";
				pdx.PrinterSettings.PrintToFile = true;
				pdx.PrinterSettings.PrintFileName = temp;
				pdx.PrintController = new StandardPrintController();
				pdx.Print();
			}

			var wmf = new WMFImage();
			using (var package = Package.Open(temp, FileMode.Open, FileAccess.Read))
			{
				const string inMemoryPackageName = "memorystream://miXps.xps";
				var packageUri = new Uri(inMemoryPackageName);
				PackageStore.AddPackage(packageUri, package);
				using (var xpsDocument = new XpsDocument(package, CompressionOption.NotCompressed, inMemoryPackageName))
				{
					var docReference = xpsDocument.GetFixedDocumentSequence().References.First();
					var doc = docReference.GetDocument(false);
					var content = doc.Pages[0];
					var fixedPage = content.GetPageRoot(false);
					wmf.Canvas = new Canvas
					{
						Width = fixedPage.Width,
						Height = fixedPage.Height,
					};
					for (var i = fixedPage.Children.Count - 1; i >= 0; i--)
					{
						var child = fixedPage.Children[i];
						fixedPage.Children.Remove(child);
						if (child is Glyphs)
						{
							var glyph = (Glyphs)child;
							var glyphRun = glyph.ToGlyphRun();
							var path = new System.Windows.Shapes.Path
							{
								Fill = glyph.Fill,
								Data = glyphRun.BuildGeometry(),
								RenderTransform = glyph.RenderTransform,
								RenderTransformOrigin = glyph.RenderTransformOrigin,
								RenderSize = glyph.RenderSize,
							};
							wmf.Canvas.Children.Insert(0, path);
						}
						else
							wmf.Canvas.Children.Insert(0, child);
					}

					if (xpsDocument.FixedDocumentSequenceReader != null)
						ReadResources(xpsDocument.FixedDocumentSequenceReader.FixedDocuments[0].FixedPages[0], wmf);

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
			catch (Exception e)
			{
				Logger.Error(e);
			}

			return wmf;
		}
		public static DrawingGroup ReadDrawing(string fileName)
		{
			var wmf = ReadWMF(fileName);
			var brush = new VisualBrush(wmf.Canvas);
			var drawingVisual = new DrawingVisual();

			using (var drawingContext = drawingVisual.RenderOpen())
			{
				drawingContext.DrawRectangle(brush, null, brush.Viewport);
				drawingContext.Close();
			}

			return drawingVisual.Drawing;
		}

		private static void ReadResources(IXpsFixedPageReader reader, WMFImage wmf)
		{
			if (wmf.Canvas == null)
				return;

			foreach (var glyph in wmf.Canvas.FindVisualChildren<Glyphs>())
			{
				var guid = new Guid(Path.GetFileNameWithoutExtension(glyph.FontUri.ToString()));
				if (!wmf.Resources.ContainsKey(guid))
				{
					var data = DeobfuscateFont(reader.GetFont(glyph.FontUri));
					wmf.Resources.Add(guid, data);
				}
				//glyph.FontUri = new Uri(guid.ToString(), UriKind.Relative);
			}
		}
		private static byte[] DeobfuscateFont(XpsFont font)
		{
			using (var stm = font.GetStream())
			{
				var dta = new byte[stm.Length];
				stm.Read(dta, 0, dta.Length);
				if (font.IsObfuscated)
				{
					//string guid = new Guid(font.Uri.GetFileName().Split('.')[0]).ToString("N");
					var guid = new Guid(Path.GetFileNameWithoutExtension(font.Uri.ToString())).ToString("N");
					var guidBytes = new byte[16];

					for (var i = 0; i < guidBytes.Length; i++)
						guidBytes[i] = Convert.ToByte(guid.Substring(i * 2, 2), 16);

					for (var i = 0; i < 32; i++)
					{
						var gi = guidBytes.Length - (i % guidBytes.Length) - 1;
						dta[i] ^= guidBytes[gi];
					}
				}
				return dta;
			}
		}

	}
}
