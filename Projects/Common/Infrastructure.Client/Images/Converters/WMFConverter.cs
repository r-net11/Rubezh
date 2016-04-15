using Controls;
using Infrastructure.Client.Images;
using System;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps.Packaging;

namespace Infrastructure.Client.Converters
{
	public static class WMFConverter
	{
		public static WMFImage ReadWMF(string fileName)
		{
			#region createTempFile
			var temp = Path.Combine(Path.GetTempPath() + Path.GetRandomFileName());
			temp = temp.Remove(temp.Length - 3) + "xps";
			#endregion
			using (PrintDocument pdx = new PrintDocument())
			{
				pdx.PrintPage += (object printSender, PrintPageEventArgs printE) =>
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
			#region WaitForPrinter

			for (int i = 0; i < 10; i++)
			{
				if (File.Exists(temp))
				{
					FileInfo fi = new FileInfo(temp);
					for (int j = 0; i < 10; i++)
					{
						if (fi.Length == 0)
						{
							System.Threading.Thread.Sleep(1000);
							fi = new FileInfo(temp);
						}
						else break;

						if ((fi.Length == 0) && (j == 9))
							throw new Exception("Печать в файл занимает слишком много времени. Либо размер файла слишком большой, либо проблема связана с XPS принтером, либо файл повреждён.");
						break;
					}
					break;
				}
				else
				{
					System.Threading.Thread.Sleep(1000);
				}
			}
			#endregion
			WMFImage wmf = new WMFImage();
			using (XpsDocument xpsDocument = new XpsDocument(temp, FileAccess.Read))
			{
				DocumentReference docReference = xpsDocument.GetFixedDocumentSequence().References.First();
				FixedDocument doc = docReference.GetDocument(false);
				PageContent content = doc.Pages[0];
				var fixedPage = content.GetPageRoot(false);
				wmf.Canvas = new Canvas()
				{
					Width = fixedPage.Width,
					Height = fixedPage.Height,
				};
				for (int i = fixedPage.Children.Count - 1; i >= 0; i--)
				{
					var child = fixedPage.Children[i];
					fixedPage.Children.Remove(child);
					if (child is Glyphs)
					{
						var glyph = (Glyphs)child;
						var glyphRun = glyph.ToGlyphRun();
						var path = new System.Windows.Shapes.Path()
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
				ReadResources(xpsDocument.FixedDocumentSequenceReader.FixedDocuments[0].FixedPages[0], wmf);
				xpsDocument.Close();
			}
			GC.Collect();
			try
			{
				File.Delete(temp);
			}
			catch
			{
			}
			return wmf;
		}
		public static DrawingGroup ReadDrawing(string fileName)
		{
			var wmf = ReadWMF(fileName);
			var brush = new VisualBrush(wmf.Canvas);
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
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
			}
		}
		private static byte[] DeobfuscateFont(XpsFont font)
		{
			using (var stm = font.GetStream())
			{
				byte[] dta = new byte[stm.Length];
				stm.Read(dta, 0, dta.Length);
				if (font.IsObfuscated)
				{
					string guid = new Guid(System.IO.Path.GetFileNameWithoutExtension(font.Uri.ToString())).ToString("N");
					byte[] guidBytes = new byte[16];
					for (int i = 0; i < guidBytes.Length; i++)
						guidBytes[i] = Convert.ToByte(guid.Substring(i * 2, 2), 16);

					for (int i = 0; i < 32; i++)
					{
						int gi = guidBytes.Length - (i % guidBytes.Length) - 1;
						dta[i] ^= guidBytes[gi];
					}
				}
				return dta;
			}
		}

	}
}
