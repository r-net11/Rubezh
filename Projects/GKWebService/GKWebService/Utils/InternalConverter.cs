#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Infrastructure.Common.Windows.Services.Content;
using RubezhAPI.GK;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

#endregion

namespace GKWebService.Utils
{
	internal static class InternalConverter
	{
		public static string RenderIcon(Guid? source, int width, int height) {
			var contentService = new ContentService("Sergey_GKOPC");
			Drawing drawing;
			if (source.HasValue) {
				drawing = contentService.GetDrawing(source.Value);
				if (drawing == null) {
					return string.Empty;
				}
			}
			else {
				return string.Empty;
			}
			drawing.Freeze();

			return InternalConverter.XamlDrawingToPngBase64String(width, height, drawing);
		}

		/// <summary>
		///     Преобразует XAML Drawing/DrawingGroup в png base64 string
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="drawing"></param>
		/// <returns>Base64 string containing png bitmap</returns>
		public static string XamlDrawingToPngBase64String(int width, int height, Drawing drawing) {
			var bitmapEncoder = new PngBitmapEncoder();
			// The image parameters...
			double dpiX = 96;
			double dpiY = 96;

			// The Visual to use as the source of the RenderTargetBitmap.
			var drawingVisual = new DrawingVisual();
			using (var drawingContext = drawingVisual.RenderOpen()) {
				drawingContext.DrawDrawing(drawing);
			}

			var bounds = drawingVisual.ContentBounds;

			var targetBitmap = new RenderTargetBitmap(
				width * 10, height * 10, dpiX, dpiY,
				PixelFormats.Pbgra32);
			drawingVisual.Transform = new ScaleTransform(width * 10 / bounds.Width, height * 10 / bounds.Height);

			targetBitmap.Render(drawingVisual);

			// Encoding the RenderBitmapTarget as an image.
			bitmapEncoder.Frames.Add(BitmapFrame.Create(targetBitmap));

			byte[] values;
			using (var str = new MemoryStream()) {
				bitmapEncoder.Save(str);
				values = str.ToArray();
			}
			return Convert.ToBase64String(values);
		}

		/// <summary>
		///     Преобразует Xaml-canvas в png изображение
		/// </summary>
		/// <param name="surface">XAML Canvas</param>
		/// <returns>Bitmap, rendered from XAML</returns>
		public static Bitmap XamlCanvasToPngBitmap(Canvas surface) {
			if (surface == null) {
				throw new ArgumentNullException("surface");
			}

			surface.LayoutTransform = null;

			// Get the size of canvas
			var size = new Size(surface.Width, surface.Height);


			// Measure and arrange the surface
			// VERY IMPORTANT
			surface.Measure(size);
			surface.Arrange(new Rect(size));

			// Create a render bitmap and push the surface to it
			var renderBitmap =
				new RenderTargetBitmap(
					(int)size.Width,
					(int)size.Height,
					96d,
					96d,
					PixelFormats.Pbgra32);
			renderBitmap.Render(surface);

			Bitmap bmp;
			// Create a file stream for saving image
			using (var stream = new MemoryStream()) {
				// Use png encoder for our data
				var encoder = new PngBitmapEncoder();
				// push the rendered bitmap to it
				encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
				// save the data to the stream
				encoder.Save(stream);

				bmp = new Bitmap(stream);
			}
			return bmp;
		}

		public static string XamlCanvasToPngBase64(Canvas surface, double width, double height) {
			if (surface == null) {
				throw new ArgumentNullException("surface");
			}
			//Grid newSurface = new Grid();

			//foreach (UIElement child in surface.Children) {
			//	var xaml = System.Windows.Markup.XamlWriter.Save(child);
			//	var deepCopy = System.Windows.Markup.XamlReader.Parse(xaml) as UIElement;
			//	newSurface.Children.Add(deepCopy);
			//}

			// Get the size of canvas
			var size = new Size(width, height);

			// Measure and arrange the surface
			// VERY IMPORTANT
			surface.Measure(size);
			surface.Arrange(new Rect(size));

			double maxWidth = width;
			double maxheight = height;

			foreach (UIElement child in surface.Children)
			{
				if (child.RenderSize.Height > maxheight)
				{
					maxheight = child.RenderSize.Height;
				}
				if (child.RenderSize.Width > maxWidth)
				{
					maxWidth = child.RenderSize.Width;
				}
			}

			// Create a render bitmap and push the surface to it
			var renderBitmap =
				new RenderTargetBitmap(
					(int)maxWidth,
					(int)maxheight,
					96d,
					96d,
					PixelFormats.Pbgra32);
			renderBitmap.Render(surface);

			// Create a file stream for saving image
			using (var stream = new MemoryStream()) {
				// Use png encoder for our data
				var encoder = new PngBitmapEncoder();
				// push the rendered bitmap to it
				encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
				// save the data to the stream
				encoder.Save(stream);

				byte[] imageBytes = stream.ToArray();
				return Convert.ToBase64String(imageBytes);
			}
		}

		/// <summary>
		///     Преобразует набор точек в путь
		/// </summary>
		/// <param name="points">Коллекция точек</param>
		/// <param name="pathKind">Тип пути - Линия(замкнутая/открытая), либо Эллипс</param>
		/// <returns></returns>
		public static string PointsToPath(PointCollection points, PathKind pathKind) {
			var enumerable = points.ToArray();
			if (!enumerable.Any()) {
				return string.Empty;
			}

			switch (pathKind) {
				case PathKind.Ellipse: {
						var radiusX = (enumerable[1].X - enumerable[0].X) / 2;
						var radiusY = (enumerable[2].Y - enumerable[1].Y) / 2;
						var center = new Point(enumerable[1].X - radiusX, enumerable[2].Y - radiusY);
						var geometry = new EllipseGeometry(center: center, radiusX: radiusX, radiusY: radiusY);
						return geometry.GetFlattenedPathGeometry().ToString(CultureInfo.InvariantCulture);
					}
				default: {
						var start = enumerable[0];
						var segments = new List<LineSegment>();
						for (var i = 1; i < enumerable.Length; i++) {
							segments.Add(new LineSegment(new Point(enumerable[i].X, enumerable[i].Y), true));
						}
						var figure = new PathFigure(start, segments, (pathKind == PathKind.ClosedLine));
						var geometry = new PathGeometry();
						geometry.Figures.Add(figure);
						return geometry.ToString(CultureInfo.InvariantCulture);
					}
			}
		}

		public static System.Drawing.Color ConvertColor(Color source) {
			var color =  System.Drawing.Color.FromArgb(source.A, source.R, source.G, source.B);
			return color;
		}

	}

	public enum PathKind
	{
		Line,
		ClosedLine,
		Ellipse
	}
}
