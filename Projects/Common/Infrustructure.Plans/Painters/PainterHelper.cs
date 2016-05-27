using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StrazhAPI.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public static class PainterHelper
	{
		public static PointCollection GetPoints(ElementBase element)
		{
			return
				element is ElementBaseShape ?
					Normalize(element.GetRectangle().TopLeft, ((ElementBaseShape)element).Points, element.BorderThickness) :
				element is ElementBaseRectangle ?
					Normalize(element.GetRectangle(), element.BorderThickness) :
					new PointCollection();
		}

		public static PointCollection GetRealPoints(ElementBase element)
		{
			if (element is ElementBaseShape)
				return ((ElementBaseShape)element).Points;
			else if (element is ElementBaseRectangle)
				return GetPoints(element.GetRectangle());
			return new PointCollection();
		}

		public static PointCollection GetPoints(Rect rect, double margin = 0)
		{
			return new PointCollection()
				{
					rect.TopLeft + new Vector(-margin, -margin),
					rect.TopRight + new Vector(margin, -margin),
					rect.BottomRight + new Vector(margin, margin),
					rect.BottomLeft + new Vector(-margin, margin)
				};
		}

		public static PointCollection Normalize(Rect rectangle, double thickness)
		{
			double shift = thickness / 2;
			var pointCollection = new PointCollection();
			pointCollection.Add(new Point(shift, shift));
			pointCollection.Add(new Point(rectangle.Width + shift, shift));
			pointCollection.Add(new Point(rectangle.Width + shift, rectangle.Height + shift));
			pointCollection.Add(new Point(shift, rectangle.Height + shift));
			return pointCollection;
		}

		public static PointCollection Normalize(Point topLeftPoint, PointCollection points, double thickness)
		{
			double shift = thickness / 2;
			var pointCollection = new PointCollection();
			foreach (var point in points)
				pointCollection.Add(new Point(point.X - topLeftPoint.X + shift, point.Y - topLeftPoint.Y + shift));
			return pointCollection;
		}

		public static Brush CreateBrush(byte[] backgroundPixels)
		{
			if (backgroundPixels == null)
				return FreezeBrush(new ImageBrush());
			using (var imageStream = new MemoryStream(backgroundPixels))
			{
				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				//bitmapImage.CreateOptions = BitmapCreateOptions.DelayCreation;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = imageStream;
				bitmapImage.EndInit();
				bitmapImage.Freeze();
				var brush = new ImageBrush(bitmapImage);
				FreezeBrush(brush);
				return brush;
			}
		}

		public static Brush FreezeBrush(Brush brush)
		{
			RenderOptions.SetBitmapScalingMode(brush, BitmapScalingMode.LowQuality);
			RenderOptions.SetCacheInvalidationThresholdMinimum(brush, 0.5);
			RenderOptions.SetCacheInvalidationThresholdMaximum(brush, 2.0);
			RenderOptions.SetCachingHint(brush, CachingHint.Cache);
			if (brush.CanFreeze)
				brush.Freeze();
			return brush;
		}
	}
}