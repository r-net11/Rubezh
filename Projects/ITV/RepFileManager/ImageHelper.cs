using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace RepFileManager
{
	public static class ImageHelper
	{
		public static Canvas XmlToCanvas(string xmlOfimage)
		{
			var canvas = new Canvas();
			try
			{
				using (var stringReader = new StringReader(xmlOfimage))
				{
					var xmlReader = XmlReader.Create(stringReader);
					canvas = (Canvas)XamlReader.Load(xmlReader);
				}
			}
			catch { }
			return canvas;
		}

		public static void XAMLToBitmap(FrameworkElement element, string fileName)
		{
			try
			{
				element.RenderTransform = new ScaleTransform(0.1, 0.1);

				element.Measure(new Size((int)element.Width, (int)element.Height));
				element.Arrange(new Rect(new Size((int)element.Width, (int)element.Height)));

				var renderTargetBitmap = new RenderTargetBitmap((int)element.ActualWidth / 10, (int)element.ActualHeight / 10, 96d, 96d, PixelFormats.Pbgra32);
				renderTargetBitmap.Render(element);

				using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
				{
					var bmpBitmapEncoder = new BmpBitmapEncoder();
					bmpBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
					bmpBitmapEncoder.Save(fileStream);
				}
			}
			catch { }
		}
	}
}