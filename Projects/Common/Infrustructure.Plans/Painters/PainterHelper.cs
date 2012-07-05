﻿using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public static class PainterHelper
	{
		public static void SetStyle(Shape shape, ElementBase element)
		{
			shape.Fill = new SolidColorBrush(element.BackgroundColor);
			shape.Stroke = new SolidColorBrush(element.BorderColor);
			shape.StrokeThickness = element.BorderThickness;
			if (element.BackgroundPixels != null)
				shape.Fill = PainterHelper.CreateBrush(element.BackgroundPixels);

		}
		public static PointCollection GetPoints(ElementBase element)
		{
			return element is ElementBaseShape ? ((ElementBaseShape)element).Points : new PointCollection();
		}
		public static Brush CreateBrush(byte[] backgroundPixels)
		{
			using (var imageStream = new MemoryStream(backgroundPixels))
			{
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = imageStream;
				bitmapImage.EndInit();

				return new ImageBrush(bitmapImage);
			}
		}
	}
}